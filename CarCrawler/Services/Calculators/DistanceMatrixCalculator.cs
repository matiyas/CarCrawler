﻿using CarCrawler.Services.Calculators.Providers.Interfaces;
using System.Numerics;

namespace CarCrawler.Services.Calculators;

internal class DistanceMatrixCalculator
{
    private readonly IDistanceMatrixProvider _provider;

    public DistanceMatrixCalculator (IDistanceMatrixProvider provider)
    {
        _provider = provider;
    }

    public DistanceMatrix? Calculate (Vector2 origin, Vector2 destination)
    {
        _provider.Origin = origin;
        _provider.Destination = destination;

        return _provider.GetDistanceMatrix();
    } 
}