﻿using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationSistarbanc : IService<ConciliationSistarbanc, ConciliationSistarbancDto>
    {
        bool DirectoryConciliation();
        void SingleFileConciliation(string fileName);
    }
}