
#include "pch.h"
#ifndef HYBRIDANOMALYDETECTOR_H_
#define HYBRIDANOMALYDETECTOR_H_

#include "SimpleAnomalyDetector.h"
#include "minCircle.h"

class HybridAnomalyDetector:public SimpleAnomalyDetector {
public:
	HybridAnomalyDetector();
	virtual ~HybridAnomalyDetector();
	//virtual void learnNormal(const TimeSeries& ts);
	//virtual vector<AnomalyReport> detect(const TimeSeries& ts);
	
	virtual float getDev(Point& pt, CorrelationType type, Line& lin_reg ,void* objectToCheckDev);
	virtual float getThreshold(CorrelationType type, void* objectToCheckDev);
	virtual void  customDelete(CorrelationType type, void* objectToDelete);
	virtual CorrelationType enoughPearsonFor(float p);
	virtual void* getObjectToCheckDev(CorrelationType type, Point** arr, size_t size);
};

#endif /* HYBRIDANOMALYDETECTOR_H_ */
