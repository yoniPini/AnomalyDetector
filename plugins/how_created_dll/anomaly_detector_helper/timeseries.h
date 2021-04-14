#include "pch.h"

#ifndef TIMESERIES_H_
#define TIMESERIES_H_

#include "anomaly_detection_util.h"
#include <vector>
#include <string>
using namespace std;

class TimeSeries {
    public:
        TimeSeries();
        TimeSeries(const char* CSVfileName);
		void LoadCSV(const char* CSVfileName);
		void Clear();
		
        const float* getValuesOf(int idx) const;
        Point** createPointsFrom(int i, int j) const;
        void freePoints(Point** arr) const;
        string getNameOf(int idx) const;
        int getIdxOf(string name) const;
        int getFeaturesAmount() const;
        int getValuesSize() const;
        
    private:
        vector<string> m_featuresNames;
        int m_featuresAmount;
        vector<vector<float>> m_featuresValues;
};



#endif /* TIMESERIES_H_ */
