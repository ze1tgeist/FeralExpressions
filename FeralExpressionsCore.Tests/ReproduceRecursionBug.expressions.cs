using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace FeralExpressionsCore.ReproduceRecursionBug
{
    public partial class IndependentSolutionDomainToDtoMapper<TDomainEntity, TDto> : IDomainToDtoMapper<TDomainEntity, TDto> where TDto : class
    {
        public static Expression<Func<IndependentSolutionDomainToDtoMapper<TDomainEntity,TDto>,TDomainEntity,TDto>> DomainEntityToDto_Expression =>
        (IndependentSolutionDomainToDtoMapper<TDomainEntity,TDto> _this, TDomainEntity domainEntity) => _this.domainToDtoConverter.Convert(domainEntity);

    }

    public partial class LegislationModelToDtoConverter : IConverter<Models.Legislation, Dtos.Legislation>
    {
        public static Expression<Func<LegislationModelToDtoConverter,Models.Legislation,Dtos.Legislation>> Convert_Expression =>
        (LegislationModelToDtoConverter _this, Models.Legislation from) => new Dtos.Legislation();
    }

}
