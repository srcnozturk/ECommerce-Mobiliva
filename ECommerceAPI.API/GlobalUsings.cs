// System namespaces
global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;

// Microsoft Extensions
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

// Swagger
global using Microsoft.OpenApi.Models;

// MediatR
global using MediatR;

// Project namespaces
global using ECommerceAPI.API.Middleware;
global using ECommerceAPI.Application.Commands;
global using ECommerceAPI.Application.Queries;
global using ECommerceAPI.Core.Models.Email;
