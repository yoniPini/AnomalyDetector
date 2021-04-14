#include "pch.h"
#include "minCircle.h"

// Get diatance between 2 points (distance is in use in std::)
double calcDistance(const Point* p1, const Point* p2) {
	double x1 = p1->x;
	double y1 = p1->y;

	double x2 = p2->x;
	double y2 = p2->y;

	double diffX = x1 - x2;
	double diffY = y1 - y2;
	return std::sqrt(diffX * diffX + diffY * diffY);
}

// Get point in the middle between 2 points
Point middle(const Point* p1, const Point* p2) {
	return Point((p1->x + p2->x) / 2, (p1->y + p2->y) / 2);
}

// Check if point is excatly on the Circumference of a circle
bool isPointSitOnCircleEdge(const Circle& c, const Point* p) {
	return std::abs(calcDistance(&c.center, p) - c.radius) <= 0.1;
}

// Check if point is within the circle surface / Circumference
bool isPointInCircle(const Circle& c, const Point* p) {
	return isPointSitOnCircleEdge(c, p) || calcDistance(&c.center, p) <= c.radius + 0.1;
}

// create circle which Circumference defined by 1 point
Circle createCircle(Point* p1) {
	return Circle(*p1, 0);
}

// create circle which Circumference defined by 2 point
// the 2 point are on its diameter
Circle createCircle(Point* p1, Point* p2) {
	Point middlePt = middle(p1, p2);
	return Circle(middle(p1, p2), calcDistance(p1, &middlePt));
}

double getMax(double x, double y) { return x > y ? x : y; }
double getMin(double x, double y) { return x < y ? x : y; }

// ensure gradient is in the range [-1, 1] (to  avoid arc(gradient) -> nan )
double ensure(double gradient) {
	return getMax(-1, getMin(1, gradient));
}

// create circle which Circumference defined by 3 point, EVEN if its not minimal
Circle createCircle(const Point* p1, const Point* p2, const Point* p3) {
    // math calculations
	double a = calcDistance(p1, p2);
	double b = calcDistance(p2, p3);
	double c = calcDistance(p1, p3);

	double s = (a + b + c) / 2;
	double Surface = std::sqrt(s * (s - a) * (s - b) * (s - c));

	double radius =  c / (4 * Surface) * a * b;

	Point middlePoint = middle(p1, p2);
	double centerToMiddlePoint = std::sqrt((a / 2) * (a / 2) + radius * radius);
	double gradient = ((double)p1->y - p2->y) / a;
	double angle = std::asin(ensure(gradient));

	double triangleAngle = std::acos(ensure((a / 2) / radius));
	double biasAngle = angle - triangleAngle;

	double option1_x = radius * std::cos(biasAngle);
	double option1_y = radius * std::sin(biasAngle);
    
    // I don't have energy to deal with calculation wheter asin/acos returned the corresponding answer
	Circle arr[] = { Circle(Point(p2->x + option1_x, p2->y + option1_y),radius) ,
					 Circle(Point(p2->x + option1_x, p2->y - option1_y),radius) ,
					 Circle(Point(p2->x - option1_x, p2->y + option1_y),radius) ,
					 Circle(Point(p2->x - option1_x, p2->y - option1_y),radius) ,
                     
					 Circle(Point(p1->x + option1_x, p1->y + option1_y),radius) ,
					 Circle(Point(p1->x + option1_x, p1->y - option1_y),radius) ,
					 Circle(Point(p1->x - option1_x, p1->y + option1_y),radius) ,
					 Circle(Point(p1->x - option1_x, p1->y - option1_y),radius) };

	for (int i = 0; i < 8; i++)
		if (isPointSitOnCircleEdge(arr[i], p3))
			return arr[i];
        
    // only to avoid gcc warning, anyway if we here there is a problem :(
	return Circle(Point(0,0) ,radius);
}

// create circle which Circumference defined by 1/2/3 point, EVEN if its not minimal
Circle createCircle(Point** points, size_t size) {
	if (size == 1)
		return createCircle(points[0]);
	if (size == 2)
		return createCircle(points[0], points[1]);
	return createCircle(points[0], points[1], points[2]);
}

// Welzl's algorithm
// get minimum circle recusivly where: (P,R) are given P-points, R=points most be excatly on Circumference
//                           P              |P|                  R                     |R|
Circle findMinCircle(Point** points, size_t pointsSize, Point** pointsOnEdge, size_t pointsOnEdgeSize) {
    // p = points[pointsSize - 1] is the the current point at P we check in this call
    
	// if |R| = 0 for sure the minimal circle (withe no limits because |R| = 0) is the point
    // (not in the original algorithm but due to implementation it's have to divie this case
    if (pointsSize == 1 && pointsOnEdgeSize == 0) 
		return Circle(*points[pointsSize - 1], 0);
	
    // if |R|=3 or (P empty and |R|>0) return the 'trivial' circle
	if (pointsOnEdgeSize == 3 || pointsSize == 0) 
		return createCircle(pointsOnEdge, pointsOnEdgeSize);
	
    // Check if current point is alredy within the circle of P \ {p}
	Circle temp = findMinCircle(points, pointsSize - 1, pointsOnEdge, pointsOnEdgeSize);
	if (isPointInCircle(temp, points[pointsSize - 1]))
		return temp;

    // Otherwise, make p as requriement of P \ {p}
    // because if p not within findMinCircle(P\{p},R) then it will be on Circumference of findMinCircle(P\{p},RU{p})
	size_t newPointsOnEdgeSize = pointsOnEdgeSize + 1;
	Point** newPointsOnEdge = new Point * [newPointsOnEdgeSize];
	int i = 0;
	for (; i < pointsOnEdgeSize; i++)
		newPointsOnEdge[i] = pointsOnEdge[i];
	newPointsOnEdge[i] = points[pointsSize - 1];

    // because we move the dynamic allocation to deeper call it's ok
	temp = findMinCircle(points, pointsSize - 1, newPointsOnEdge, newPointsOnEdgeSize);

	delete[] newPointsOnEdge;

	return temp;
}

Circle findMinCircle(Point** points, size_t size) {
    // Use Welzl's algorithm
	return findMinCircle(points, size, nullptr, 0);
}