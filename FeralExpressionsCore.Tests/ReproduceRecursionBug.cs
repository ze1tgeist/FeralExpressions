using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.ReproduceRecursionBug
{
    public interface IConverter<TFrom, TTo> where TTo : class
    {
        TTo Convert(TFrom from);
    }

    public interface IDomainToDtoMapper<TDomainEntity, TDto> where TDto : class
    {
        TDto DomainEntityToDto(TDomainEntity domainEntity);
    }
    public partial class IndependentSolutionDomainToDtoMapper<TDomainEntity, TDto> : IDomainToDtoMapper<TDomainEntity, TDto> where TDto : class
    {
        private readonly IConverter<TDomainEntity, TDto> domainToDtoConverter;

        public IndependentSolutionDomainToDtoMapper(IConverter<TDomainEntity, TDto> domainToDtoConverter)
        {
            this.domainToDtoConverter = domainToDtoConverter;
        }

        public TDto DomainEntityToDto(TDomainEntity domainEntity) => this.domainToDtoConverter.Convert(domainEntity);

    }
    namespace Models
    {
        public class Legislation
        {

        }
    }
    namespace Dtos
    {
        public class Legislation
        {

        }
    }

    public partial class LegislationModelToDtoConverter : IConverter<Models.Legislation, Dtos.Legislation>
    {
        public Dtos.Legislation Convert(Models.Legislation from) => new Dtos.Legislation();
    }

}
