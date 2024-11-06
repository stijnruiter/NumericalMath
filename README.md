# Numerical Analysis from scratch

This repository is my implementation of some basic numerical analysis topics written in C#/.NET. 
I intend to not use any libraries to start with, thus I do intend to write the Linear Algebra functionality
on my own as well. 

I know that there are various libraries out there, that will most likely do a much better job at
executing all the necessary computations, but now I am forced to learn some more aspects of the 
computational mathematics field.

## Implemented

- Dense Column/Row vector data structure
	- Underlying array data structure
- Dense Matrix data structure (row-major only)
	- Jagged array of arrays, for more flexibility down the line compared to [,] arrays
- Some simple matrix vector arithmetics (nothing optimized)
	- Operators for scalar/vector/matrix arithmetics
- _LU_ factorization (Doolittle method with and without partial pivoting)
- Solving the system **Ax=b** and **AX=B** using LU factorization
- Determinant and inverse of matrix, using the LU factorization
- First two approximation of Ordinary Differential Equations (ODE) using the Finite Difference Method (as unit tests)
	- Poisson's equation
	- Helmholtz equation


## Roadmap

### Numerics

- Rational numbers
- Derivatives
- Integrals

### Linear Algebra

- Cholesky factorization
- Sparse matrices

### Numerical Analysis

- Generic expression of ODE/PDE
- Finite Difference Methods, 1D/2D implementation
- Finite Element Methods
	- Create simple geometeries
	- 1D implementation
	- 2D imdplementation
	- Test functions
- Finite Volume Methods
- Material Point Method
	- Legrange Polynomials
	- B-splines