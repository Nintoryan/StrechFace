using System.Collections.Generic;
using System.Linq;
using Ketchapp.MayoAPI.Dto;

namespace Ketchapp.Internal.Configuration
{
    public static class ConfigurationRelatedMapper
    {
        public static List<GameConfiguration.ConfigurationRelated> MapConfigurationRelatedFromDtos(this List<ConfigurationRelatedDto> dto)
        {
            return dto?.Select(c => c.MapConfigurationRelatedFromDto()).ToList();
        }

        public static GameConfiguration.ConfigurationRelated MapConfigurationRelatedFromDto(this ConfigurationRelatedDto dto)
        {
            return new GameConfiguration.ConfigurationRelated
            {
                ManifestType = dto.ManifestType == null ? 0 : (int)dto.ManifestType,
                Name = dto.Name,
                Value = dto.Value
            };
        }
    }
}