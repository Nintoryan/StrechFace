using System.Collections.Generic;
using System.Linq;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoAPI.Dto;

namespace Ketchapp.Editor.Utils
{
    internal static class VersionningMapper
    {
        public static SDKVersion MapFromDto(this SDKVersionDto dto)
        {
            return new SDKVersion()
            {
                Changelog = dto.Changelog,
                ToolVersionId = dto.ToolVersionId,
                Extension = dto.Extension,
                Name = dto.Name,
                UnityPackageRegistryName = dto.UnityPackageRegistryName,
                UnityPackageRegistryVersion = dto.UnityPackageRegistryVersion,
                PluginPaths = dto.PluginPaths,
                PackageType = dto.PackageType,
                Type = (SDKType)dto.Type,
                Version = dto.Version
            };
        }

        public static List<SDKVersion> MapFromDtos(this List<SDKVersionDto> dtos)
        {
            return dtos?.Select(d => d.MapFromDto()).ToList();
        }
    }
}
