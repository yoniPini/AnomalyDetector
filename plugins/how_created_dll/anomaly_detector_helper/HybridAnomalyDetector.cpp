#include "pch.h"
#include "HybridAnomalyDetector.h"

#include <cmath> /* std::sqrt(float) */
using std::sqrt;

HybridAnomalyDetector::HybridAnomalyDetector() {}

HybridAnomalyDetector::~HybridAnomalyDetector() {}
	
/*void HybridAnomalyDetector::learnNormal(const TimeSeries& ts) {
	// learn normal of linear reg
	this->SimpleAnomalyDetector::learnNormal(ts);
}
vector<AnomalyReport> HybridAnomalyDetector::detect(const TimeSeries& ts){
	return this->SimpleAnomalyDetector::detect(ts);
	// minimal circle
}*/
void HybridAnomalyDetector::customDelete(CorrelationType type, void* objectToDelete) {
	if (type == MinimalCircle)
		delete ((Circle*)objectToDelete);
	else
		this->SimpleAnomalyDetector::customDelete(type, objectToDelete);
}
void* HybridAnomalyDetector::getObjectToCheckDev(CorrelationType type, Point** arr, size_t size) {
	Circle* c = new Circle(Point(0,0),0);
	*c = findMinCircle(arr, size);
	return c;
}
CorrelationType HybridAnomalyDetector::enoughPearsonFor(float p) {
	p = std::abs(p);
	if (p < 0.5)
		return None;
	//if (p >= 0.9) return LinearReg;
	return MinimalCircle;
}

float HybridAnomalyDetector::getDev(Point& pt, CorrelationType type, Line& lin_reg, void* objectToCheckDev) {
	if (type == MinimalCircle) {
			Circle* c = (Circle*) objectToCheckDev;
			float diffX = c->center.x - pt.x;
			float diffY = c->center.y - pt.y;
			return std::sqrt(diffX * diffX + diffY * diffY);
	}
	return this->SimpleAnomalyDetector::getDev(pt, type, lin_reg, objectToCheckDev);
}
float HybridAnomalyDetector::getThreshold(CorrelationType type, void* objectToCheckDev) {
	if ( type == MinimalCircle)
		return ((Circle*)objectToCheckDev)->radius;
	return this->SimpleAnomalyDetector::getThreshold(type, objectToCheckDev);
}