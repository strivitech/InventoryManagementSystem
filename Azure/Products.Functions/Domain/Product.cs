﻿namespace Products.Functions.Domain;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}