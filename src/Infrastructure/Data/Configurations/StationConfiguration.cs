using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class StationConfiguration : IdentityConfigurationBase<Station>
    {
        protected override void AddCustomConfiguration(EntityTypeBuilder<Station> builder)
        { }
    }
}