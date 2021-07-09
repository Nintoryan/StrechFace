using System;
using System.Collections.Generic;
using System.Linq;
using Ketchapp.MayoAPI.Dto;
using UnityEngine;

namespace Ketchapp.Internal.Configuration
{
    public static class MediationAdapterMapper
    {
        public static MediationAdapter MapFromDto(this MediationAdapterDto dto)
        {
            return new MediationAdapter()
            {
                PodInfo = dto.PodInfo.MapFromDtos(),
                AndroidPackagesInfos = dto.AndroidPackagesInfos.MapFromDtos(),
            };
        }

        public static List<MediationAdapter> MapFromDtos(this List<MediationAdapterDto> dtos)
        {
            return dtos.Select(a => a.MapFromDto()).ToList();
        }
    }

    public static class PodInfoMapper
    {
        public static IosPods MapFromDto(this IosPodDto dto)
        {
            return new IosPods()
            {
                IosPod = new IosPod()
                {
                    Name = dto.Name,
                    Sources = new Sources()
                    {
                        Source = dto.Source
                    },
                    Version = dto.Version
                }
            };
        }

        public static List<IosPods> MapFromDtos(this List<IosPodDto> dtos)
        {
            return dtos.Select(d => d.MapFromDto()).ToList();
        }
    }

    public static class AndroidPackageMapper
    {
        public static AndroidPackages MapFromDto(this AndroidPackageDto dto)
        {
            var package = new AndroidPackages()
            {
                AndroidPackage = new AndroidPackage()
                {
                    Spec = dto.Spec
                }
            };

            if (!string.IsNullOrEmpty(dto.Repository))
            {
                package.AndroidPackage.Repositories = new Repositories()
                {
                    Repository = dto.Repository
                };
            }
            else
            {
                package.AndroidPackage.Repositories = null;
            }

            return package;
        }

        public static List<AndroidPackages> MapFromDtos(this List<AndroidPackageDto> dtos)
        {
            return dtos.Select(a => a.MapFromDto()).ToList();
        }
    }
}
