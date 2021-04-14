//
// Ehud Wasserman
#include "pch.h"
// Welzl's algorithm: https://en.wikipedia.org/wiki/Smallest-circle_problem
// Calculate radius by triangle edges: https://he.wikipedia.org/wiki/%D7%9E%D7%A2%D7%92%D7%9C_%D7%97%D7%95%D7%A1%D7%9D#%D7%9E%D7%A2%D7%92%D7%9C_%D7%A2%D7%95%D7%98%D7%A3_%D7%9E%D7%99%D7%A0%D7%99%D7%9E%D7%9C%D7%99

#ifndef MINCIRCLE_H_
#define MINCIRCLE_H_

#include "anomaly_detection_util.h" // Point
#include <cmath> // I added

using namespace std;

// ------------ DO NOT CHANGE -----------

class Circle {
public:
	Point center;
	float radius;
	Circle(Point c, float r) :center(c), radius(r) {}
};
// --------------------------------------

// implement in cpp file
Circle findMinCircle(Point** points, size_t size);


#endif /* MINCIRCLE_H_ */
