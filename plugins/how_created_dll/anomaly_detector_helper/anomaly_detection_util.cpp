/**
 * anomaly_detection_util.cpp
 *
 * Author: Ehud Wasserman  
 */
#include "pch.h"
#include "anomaly_detection_util.h"
#include <cmath> /* std::sqrt(float), std::abs(FLOAT!!) */

using std::sqrt;
using std::abs;

float avg(float* x, int size);
float var(float* x, int size);
float cov(float* x, float* y, int size);
float pearson(float* x, float* y, int size);
Line linear_reg(Point** points, int size);
float dev(Point p, Point **points, int size);
float dev(Point p, Line l);


/** Calc average of float array. */
float avg(float* x, int size) {
    // if (size == 0) return 0;
    float sum = 0;
    for (int i = 0; i < size; i++) {
        sum += x[i];
    }
    return sum / size;
}

/** Get the variance of X and Y. */
float var(float* x, int size) {
    float xAvg = avg(x, size);
    float* xPowTwo = new float[size];
    for (int i = 0; i < size; i++) {
        xPowTwo[i] = x[i] * x[i];
    }
    float xPowTwoAvg = avg(xPowTwo, size);
    delete[] xPowTwo;
    return xPowTwoAvg - xAvg * xAvg;
}

/** Get the covariance of X and Y. */
float cov(float* x, float* y, int size) {
    float xAvg = avg(x, size);
    float yAvg = avg(y, size);
    float* mul = new float[size];
    for (int i = 0; i < size; i++) {
        mul[i] = (x[i] - xAvg) * (y[i] - yAvg);
    }
    float covXY = avg(mul, size);
    delete[] mul;
    return covXY;
}

/** Get the Pearson correlation coefficient of X and Y . */
float pearson(float* x, float* y, int size) {
    float varX = var(x, size);
    float varY = var(y, size);
    float covXY = cov(x, y, size);

    /* For another edge cases of same points
    if (sqrt(varX * varY) == 0) { // 1 0 -1
        if (abs(varX - varY) < 0.00001) return 1;
        if (abs(varX + varY) < 0.00001) return -1;
        return 0;
    }
    */
    return covXY / sqrt(varX * varY);
}

/** Perform a linear regression and return the line equation.
    points is array of pointers to point.                     */
Line linear_reg(Point** points, int size) {
    // points -> x,y
    float* x = new float[size];
    float* y = new float[size];
    for (int i = 0; i < size; i++) {
        x[i] = points[i]->x;
        y[i] = points[i]->y;
    }

    float covXY = cov(x, y, size);
    float varX = var(x, size);
    float xAvg = avg(x, size);
    float yAvg = avg(y, size);

    float a = covXY / varX;
    float b = yAvg - a * xAvg;

    /*
    // vertical line -> a=infinty ,b= -1 * (constant x value)
    if ( abs(varX) < 0.00001){
        a = INFINITY;
        b = -xAvg;
    }
    // avoid represntion of -0 in the field Line.b(float IEEE)
    if ( b == 0) b =0;
    */
    delete[](x, y);
    return Line(a, b);
}

/** Return the deviation between point p and the line equation of the points.
    points is array of pointers to point.                                     */
float dev(Point p, Point** points, int size) {
    return dev(p, linear_reg(points, size));
}

/** Returns the deviation between point p and the line. */
float dev(Point p, Line l) {
    /*
    // vertical line -> a=infinty ,b= -1 * (constant x value)
    if (l.a == INFINITY)
        return abs(-l.b - p.x);
    */

    // return |f(x) - y| = distance between (x,y) (x,f(x))
    return abs(l.f(p.x) - p.y);
}
