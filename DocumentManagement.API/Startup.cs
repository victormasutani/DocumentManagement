﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using DocumentManagement.Application.AutoMapper.Profiles;
using DocumentManagement.API.Handlers;
using DocumentManagement.Application.Interfaces;
using DocumentManagement.Application.Services;
using DocumentManagement.Domain.Interfaces;
using DocumentManagement.Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DocumentManagement.API.Middlewares;
using DocumentManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using DocumentManagement.Infrastructure.Storage;

namespace DocumentManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
        }

        public IConfiguration Configuration { get; }
        private IServiceProvider ServiceProvider { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(o => o.AddPolicy("Policy", builder =>
            {
                builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowCredentials();
            }));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("document-management-v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Document Management API",
                    Version = "v1"
                });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}.xml"));
            });

            Mapper.Initialize(cfg => cfg.AddProfile<DocumentProfile>());
            Mapper.AssertConfigurationIsValid();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentStorage, DocumentStorage>();

            services.AddDbContext<DatabaseContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("DatabaseContext")));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/documentManagement-{Date}.txt");
            app.UseLoggingMiddleware();
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseCors("Policy");

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/document-management-v1/swagger.json", "Document Management API");
            });

            app.UseWelcomePage("/");
        }
    }
}