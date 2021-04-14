// Ehud Wasserman
#include "pch.h"
#include "timeseries.h"
#include <cstring> //strcpy, strtok
#include <fstream>

// TimeSeries is object that contains fields(columns-title-string) and timesteps(rows/values)
// example: TimeSeries ts("f.csv"); where f.csv contains:
// A,B,...,W
// 1,3,...,2.9
// 2,4,...,7.8
//
// ts.getValuesOf( ts.getIdxOf("B") )[0] == ts.getValuesOf(1)[0] == 3

void TimeSeries::Clear() {
	m_featuresNames.clear();
	m_featuresAmount = 0;
	m_featuresValues.clear();
}
TimeSeries::TimeSeries() {
	m_featuresAmount = 0;
}
// constructor - get csv file name and store it data at 'table' : m_featuresNames, m_featuresValues
TimeSeries::TimeSeries(const char* CSVfileName) {
	LoadCSV(CSVfileName);
}
void TimeSeries::LoadCSV(const char* CSVfileName) {
    ifstream inputFile(CSVfileName);
    string str;
    // first line is the title
    inputFile >> str;
    
    // strtok change the given string and put '\0' instead of the delitimer
    char* arr = new char[str.size()+1];
    strcpy(arr, str.c_str());
    char * token = strtok(arr, ",");
    while (token) {
        // take the field name and add to the end of the vector
        m_featuresNames.push_back(string(token));
        token = strtok(NULL, ",");
    }
    delete[] arr;
    m_featuresAmount = m_featuresNames.size();
    
    m_featuresValues = vector<vector<float>>(m_featuresAmount);
    float x;
    int i = 0;
    
    // take float number, ignore the ','(or '\n') and go to next float number
    inputFile >> x;
    inputFile.ignore();
    do {
       m_featuresValues[i].push_back(x);
       // (decide to which feature to insert the value)
       i = (i + 1) % m_featuresAmount;
       inputFile >> x;
       inputFile.ignore();
    } while(!inputFile.eof());
    inputFile.close();
}

// get the inner array of the idx'th vector
const float* TimeSeries::getValuesOf(int idx) const {
    return m_featuresValues[idx].data();
}

// get points array where x values from i'th vector and y values from j'th vector
// this is new allocation and should be free by this->freePoints()
Point** TimeSeries::createPointsFrom(int i, int j) const {
    int valuesSize = getValuesSize();
    Point** arr = new Point*[valuesSize];
    for (int k = 0; k < valuesSize; k++)
        arr[k] = new Point(m_featuresValues[i][k], m_featuresValues[j][k]);
    return arr;
}

// free point array that was allocated by this->createPointsFrom(), 
// therefore, no size is needed because it is known within this TimeSeries
void TimeSeries::freePoints(Point** arr) const {
    int valuesSize = getValuesSize();
    for (int i = 0; i < valuesSize; i++)
        delete arr[i];
    delete[] arr;
}

// get the name/title(string) of the field(column)
string TimeSeries::getNameOf(int idx) const {
    return m_featuresNames[idx];
}

// get the idx of the field according to its name
int TimeSeries::getIdxOf(string name) const {
    // ): cannot use map since 'return myMap["name"];' cannot be in const method
    for (int i = 0; i < m_featuresAmount; i++)
        if (m_featuresNames[i] == name)
            return i;
    return -1;
}

// return num of fields in this TimeSeries
int TimeSeries::getFeaturesAmount() const {
    return m_featuresAmount;
}

// return num of values(time steps/ rows) each feature has
int TimeSeries::getValuesSize() const {
    return m_featuresValues[m_featuresAmount - 1].size();
}
