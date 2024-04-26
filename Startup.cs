using System;

public void ConfigureServices(IServiceCollection services)
{
    services.Configure<FormOptions>(options =>
    {
        options.ValueCountLimit = 200; // 200 items max
        options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB max len form data
    });
