using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceFaq : BaseService<Faq, FaqDto>, IServiceFaq
    {
        
        public ServiceFaq(IRepositoryFaq repository)
            : base(repository)
        {
        }

        public override IQueryable<Faq> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override FaqDto Converter(Faq entity)
        {
            if (entity == null) return null;

            return new FaqDto
            {
                Id = entity.Id,
                Order = entity.Order,
                Question = entity.Question,
                Answer = entity.Answer
            };
        }

        public override Faq Converter(FaqDto entity)
        {
            if (entity == null) return null;

            return new Faq
            {
                Id = entity.Id,
                Order = entity.Order,
                Question = entity.Question,
                Answer = entity.Answer
            };
        }

        public override FaqDto Create(FaqDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Question.Equals(entity.Question, StringComparison.InvariantCultureIgnoreCase)).Any())
                throw new BusinessException(CodeExceptions.FAQ_QUESTION_ALREADY_USED);

            return base.Create(entity, returnEntity);
        }

        public override void Edit(FaqDto entity)
        {
            if (Repository.AllNoTracking(s => s.Question.Equals(entity.Question, StringComparison.InvariantCultureIgnoreCase) && s.Id != entity.Id).Any())
                throw new BusinessException(CodeExceptions.FAQ_QUESTION_ALREADY_USED);

            var entity_db = Repository.GetById(entity.Id);

            entity_db.Order = entity.Order;
            entity_db.Question = entity.Question;
            entity_db.Answer = entity.Answer;

            Repository.Edit(entity_db);
            Repository.Save();
        }

        public IEnumerable<FaqDto> GetDataForTable(FaqFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => //sc.Order.ToString().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Question.ToLower().Contains(filters.GenericSearch.ToLower()));


            if (filters.Order != 0)
                query = query.Where(sc => sc.Order.Equals(filters.Order));

            if (!string.IsNullOrEmpty(filters.Question))
                query = query.Where(sc => sc.Question.ToLower().Contains(filters.Question.ToLower()));


            if (filters.SortDirection == SortDirection.Asc)
                query = query.OrderByStringProperty(filters.OrderBy);
            else
                query = query.OrderByStringPropertyDescending(filters.OrderBy);
            
            return query.Select(t => new FaqDto { Id = t.Id, Order = t.Order, Question = t.Question, Answer = t.Answer }).ToList();
        }

        
    }
}
