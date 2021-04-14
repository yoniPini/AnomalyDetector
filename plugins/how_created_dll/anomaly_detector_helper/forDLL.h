#include "pch.h"
#include "minCircle.h"
#include "timeseries.h"
#include "SimpleAnomalyDetector.h"
#include "HybridAnomalyDetector.h"
#include <Windows.h>
// 
//#pragma once

// basic functions to be inlined in c# dll runtime library
// if you extends in cpp tha SimpleAnomalyDetector class, it should work
// (but check for compatibility for object of correlation that are not line\circle in the c# library)

extern "C" _declspec(dllexport) float getCircleX_cpp(Circle* c) {
	return c->center.x;
}
extern "C" _declspec(dllexport) float getCircleY_cpp(Circle * c) {
	return c->center.y;
}
extern "C" _declspec(dllexport) float getCircleR_cpp(Circle * c) {
	return c->radius;
}
extern "C" _declspec(dllexport) void del_SimpleAnomalyDetector(SimpleAnomalyDetector * ad) {
	delete ad;
}
// del_SimpleAnomalyDetector should be called for the return value
extern "C" _declspec(dllexport) void* getSimpleAnomalyDetector_cpp(const char* csvfile, float linear_threshold) {
	auto ad = new SimpleAnomalyDetector();
	ad->setCorrelationThreshold(linear_threshold);
	ad->learnNormal(TimeSeries(csvfile));
	return ad;
}
extern "C" _declspec(dllexport) float Lin_Reg_Simple_Default_Threshold() {
	return SimpleAnomalyDetector().getCorrelationThreshold();
}
// del_SimpleAnomalyDetector should be called for the return value
extern "C" _declspec(dllexport) void* getHybridAnomalyDetector_cpp(const char* csvfile, float linear_threshold) {
	auto ad = new HybridAnomalyDetector();
	ad->setCorrelationThreshold(linear_threshold);
	ad->learnNormal(TimeSeries(csvfile));
	return ad;
}
extern "C" _declspec(dllexport) float Lin_Reg_Hybrid_Default_Threshold() {
	return HybridAnomalyDetector().getCorrelationThreshold();
}
extern "C" _declspec(dllexport) void setLin_Reg_Threshold(SimpleAnomalyDetector* ad, float p) {
	ad->setCorrelationThreshold(p);
}
extern "C" _declspec(dllexport) float getLin_Reg_Threshold(SimpleAnomalyDetector * ad) {
	return ad->getCorrelationThreshold();
}
extern "C" _declspec(dllexport) void del_AnomalyReportArray(AnomalyReport * ar) {
	delete[] ar;
}
// del_AnomalyReportArray should be called for the return value
extern "C" _declspec(dllexport) void* getAnomalyReport_cpp(SimpleAnomalyDetector* ad, const char* csvfile) {
	auto ar = ad->detect(TimeSeries(csvfile));
	int arsize = ar.size();
	AnomalyReport* arArray = new AnomalyReport[arsize];
	for (int i = 0; i < arsize; i++)
		arArray[i] = ar[i];
	return arArray;
}
extern "C" _declspec(dllexport) int getAnomalyReportLastSize_cpp(SimpleAnomalyDetector * ad) {
	return ad->lastReport.size();
}
//return value should be converted to c# string
extern "C" _declspec(dllexport) const char* getFeaturesInReport(AnomalyReport * ar, int idx) {
	return ar[idx].description.c_str();
}
extern "C" _declspec(dllexport) int getTimeStepInReport(AnomalyReport * ar, int idx) {
	return ar[idx].timeStep;
}
extern "C" _declspec(dllexport) int getNormalModelSize(SimpleAnomalyDetector * ad) {
	return ad->cf.size();
}
//return value should be converted to c# string
extern "C" _declspec(dllexport) const char* getNormalModelFeature1(SimpleAnomalyDetector * ad, int idx) {
	return ad->cf[idx].feature1.c_str();
}
//return value should be converted to c# string
extern "C" _declspec(dllexport) const char* getNormalModelFeature2(SimpleAnomalyDetector * ad, int idx) {
	return ad->cf[idx].feature2.c_str();
}
extern "C" _declspec(dllexport) float getNormalModelCorrelation(SimpleAnomalyDetector * ad, int idx) {
	return ad->cf[idx].corrlation;
}
extern "C" _declspec(dllexport) bool getNormalModelIsLinearReg(SimpleAnomalyDetector * ad, int idx) {
	return ad->cf[idx].type == LinearReg;
}
extern "C" _declspec(dllexport) bool getNormalModelIsMinimalCircle(SimpleAnomalyDetector * ad, int idx) {
	return ad->cf[idx].type == MinimalCircle;
}
extern "C" _declspec(dllexport) void* getNormalModelgetMinimalCircle(SimpleAnomalyDetector * ad, int idx) {
	return ad->cf[idx].objectToCheckDev;
}
extern "C" _declspec(dllexport) void* getNormalModelgetLinearReg(SimpleAnomalyDetector * ad, int idx) {
	return &(ad->cf[idx].lin_reg);
}
extern "C" _declspec(dllexport) float getA_Line(Line * l) {
	return l->a;
}
extern "C" _declspec(dllexport) float getB_Line(Line * l) {
	return l->b;
}
