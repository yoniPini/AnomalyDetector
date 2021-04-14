// Ehud Wasserman
#include "pch.h"
#include "SimpleAnomalyDetector.h"
#include <cmath>
#include <iostream>

// get the max dev from the line in the point arr
float maxDev(Point ** arr, int size, Line line) {
    float currentMaxDev = 0;
    for (int i = 0; i < size; i++) {
        float d = dev(*arr[i], line);
        currentMaxDev = d > currentMaxDev ? d : currentMaxDev;
    }
    return currentMaxDev;
}

SimpleAnomalyDetector::SimpleAnomalyDetector() {}

SimpleAnomalyDetector::~SimpleAnomalyDetector() {	
	for(auto currCheck : cf) {
		this->customDelete(currCheck.type, currCheck.objectToCheckDev);
	}
}

// learn normal correlatedFeatures and their threshold according to the TimeSeries
// meaning update vector<correlatedFeatures> cf
void SimpleAnomalyDetector::learnNormal(const TimeSeries& ts){
    // forget prev information?
    cf.clear();
    
    int featuresAmount = ts.getFeaturesAmount();
    int valuesSize = ts.getValuesSize();
    
    // decide which feature is correlative with whom the most
	vector<int> featuresCorrelationWith(featuresAmount);
    vector<float> featuresCorrelationValue(featuresAmount);
    vector<CorrelationType> featuresCorrelationType(featuresAmount);
    
    for (int i = 0; i < featuresAmount; i++) {
        // -1 == not correlation found yet
        featuresCorrelationWith[i] = -1;
    }
    
    for (int i = 0; i < featuresAmount; i++) {
        // consider only A-C not C-A, but it could even be A-C B-C C-D
        for (int j = i + 1; j < featuresAmount; j++) {
            float* iValues = (float*) ts.getValuesOf(i);
            float* jValues = (float*) ts.getValuesOf(j);
            float p = pearson(iValues, jValues, valuesSize);
            // (case1||case2) j is the most correlative feature that found with i
            bool case1 = featuresCorrelationWith[i] == -1;
            bool case2 = std::abs(p) > std::abs(featuresCorrelationValue[i]);
            // require also |p|>0.9 , meaning there is a really connection between feature i to feature j
            if (std::abs(p) >= getCorrelationThreshold() && (case1 || case2)) {
                featuresCorrelationWith[i] = j;
                featuresCorrelationValue[i] = p;
				featuresCorrelationType[i] = LinearReg;
            } else if (std::abs(p) < getCorrelationThreshold() && this->enoughPearsonFor(p) != None && (case1 || case2)) {
				featuresCorrelationType[i] = this->enoughPearsonFor(p);
				featuresCorrelationWith[i] = j;
                featuresCorrelationValue[i] = p;
			} 
        }
    }
    
    
    // for the correlative pairs, calc the lin_reg and the acceptable threshold from their line
    vector<Line> featuresLinearReg(featuresAmount);
    vector<void*> featuresObjectToCheckDev(featuresAmount);
    vector<float> featuresThreshold(featuresAmount);
    for (int i = 0; i < featuresAmount; i++) {
        int iCorrelativeWith = featuresCorrelationWith[i];
		if (iCorrelativeWith == -1) continue;
		Point** arr = ts.createPointsFrom(i, iCorrelativeWith);
        if (featuresCorrelationType[i] == LinearReg) {
            featuresLinearReg[i] = linear_reg(arr, valuesSize);
            // threshold is 1.1 times the most far(in abs) point of(featureI,featureJ) from the lin_reg
            featuresThreshold[i] = 1.1 * maxDev(arr, valuesSize, featuresLinearReg[i]);
        } else {
			featuresObjectToCheckDev[i] = this->getObjectToCheckDev(featuresCorrelationType[i], arr, valuesSize);
			featuresThreshold[i] = 1.1 * this->getThreshold(featuresCorrelationType[i], featuresObjectToCheckDev[i]);
		}
        ts.freePoints(arr);
    }
    
    
    // write the featuresCorrelationWith, featuresCorrelationValue, featuresLinearReg, featuresThreshold
    // vectors into the member vector<correlatedFeatures> cf
    for (int i = 0; i < featuresAmount; i++) {
        int iCorrelativeWith = featuresCorrelationWith[i];
        if (iCorrelativeWith != -1) {
            string iName = ts.getNameOf(i);
            string jName = ts.getNameOf(iCorrelativeWith);
            correlatedFeatures x = {iName, jName, featuresCorrelationValue[i], featuresLinearReg[i],
									featuresThreshold[i], featuresCorrelationType[i] , featuresObjectToCheckDev[i]};
            cf.push_back(x);
        }
    }
}

// return vector<AnomalyReport> according to the learnNormal that was called earlier
vector<AnomalyReport> SimpleAnomalyDetector::detect(const TimeSeries& ts){
    vector<AnomalyReport> report;
	lastReport.clear();
    // for each correlatedFeatures
    for(auto currCheck : cf) {
        int i = ts.getIdxOf(currCheck.feature1);
        int j = ts.getIdxOf(currCheck.feature2);
        int valuesSize = ts.getValuesSize();
        // check each data row (values) of the feature i,j that aren't to far from the linear_reg
        for (int k = 0; k < valuesSize; k++) {
            Point p(ts.getValuesOf(i)[k], ts.getValuesOf(j)[k]);
			
            float currDev = this->getDev(p, currCheck.type, currCheck.lin_reg, currCheck.objectToCheckDev);
            // if the point of the value of i,j in timestep(=idx of DATA ROW NUMBER+1 ==k+1) is too far,
            // add it to the report 
            if (currDev > currCheck.threshold) {
                long timeStep = k;//+1;
                report.push_back(AnomalyReport(currCheck.feature1 + "," + currCheck.feature2, timeStep));
                lastReport.push_back(AnomalyReport(currCheck.feature1 + "," + currCheck.feature2, timeStep));
            }
        }
    }
	lastTimeStepsAmount = ts.getValuesSize();
    return report;
}

float SimpleAnomalyDetector::getDev(Point& pt, CorrelationType type, Line& lin_reg ,void* objectToCheckDev) {
	switch (type) {
		case LinearReg: return dev(pt, lin_reg);
		default: return 0;
	}
}
float SimpleAnomalyDetector::getThreshold(CorrelationType type, void* objectToCheckDev) {
	return 0;
}
void SimpleAnomalyDetector::customDelete(CorrelationType type, void* objectToDelete) {
	return;
}
CorrelationType SimpleAnomalyDetector::enoughPearsonFor(float p) {
	return None;
}
void* SimpleAnomalyDetector::getObjectToCheckDev(CorrelationType type, Point** arr, size_t size) {
	return nullptr;
}
float SimpleAnomalyDetector::getCorrelationThreshold() { return m_CorrelationThreshold;}
void SimpleAnomalyDetector::setCorrelationThreshold(float p){m_CorrelationThreshold = p;}
