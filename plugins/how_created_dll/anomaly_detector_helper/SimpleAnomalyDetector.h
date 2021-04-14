#include "pch.h"

#ifndef SIMPLEANOMALYDETECTOR_H_
#define SIMPLEANOMALYDETECTOR_H_

#include "anomaly_detection_util.h"
#include "minCircle.h"
#include "AnomalyDetector.h"
#include <vector>
#include <algorithm>
#include <string.h>
#include <math.h>

typedef enum {LinearReg = 0, MinimalCircle = 1, None = 2} CorrelationType;
struct correlatedFeatures{
	string feature1,feature2;  // names of the correlated features
	float corrlation;
	Line lin_reg;
	float threshold;
	CorrelationType type;
	void* objectToCheckDev;
};


class SimpleAnomalyDetector:public TimeSeriesAnomalyDetector{
public:
	vector<correlatedFeatures> cf;
	vector<AnomalyReport> lastReport;
	int lastTimeStepsAmount;

	SimpleAnomalyDetector();
	virtual ~SimpleAnomalyDetector();

	virtual void learnNormal(const TimeSeries& ts);
	virtual vector<AnomalyReport> detect(const TimeSeries& ts);
	
	virtual float getDev(Point& pt, CorrelationType type, Line& lin_reg ,void* objectToCheckDev);
	virtual float getThreshold(CorrelationType type, void* objectToCheckDev);
	virtual void  customDelete(CorrelationType type, void* objectToDelete);
	virtual CorrelationType enoughPearsonFor(float p);
	float getCorrelationThreshold();
	void setCorrelationThreshold(float p);
	virtual void* getObjectToCheckDev(CorrelationType type, Point** arr, size_t size);
	
	float m_CorrelationThreshold = 0.9;
	vector<correlatedFeatures> getNormalModel(){
		return cf;
	}

};



#endif /* SIMPLEANOMALYDETECTOR_H_ */
