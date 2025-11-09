using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BloggingAgent.Models.Domain
{
    public interface IRepository<TAggregate> where TAggregate : IAggregateRoot
    {
        Task<TAggregate> GetByIdAsync(int id);
        Task<IEnumerable<TAggregate>> GetAllAsync();
        Task<IEnumerable<TAggregate>> FindAsync(Expression<Func<TAggregate, bool>> predicate);
        Task<TAggregate> SingleOrDefaultAsync(Expression<Func<TAggregate, bool>> predicate);
        Task AddAsync(TAggregate aggregate);
        Task UpdateAsync(TAggregate aggregate);
        Task DeleteAsync(TAggregate aggregate);
        Task<bool> ExistsAsync(Expression<Func<TAggregate, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TAggregate, bool>> predicate = null);
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        bool HasActiveTransaction { get; }
    }

    public abstract class RepositoryBase<TAggregate> : IRepository<TAggregate>
        where TAggregate : AggregateRoot, IAggregateRoot
    {
        protected readonly IUnitOfWork UnitOfWork;

        IUnitOfWork IRepository<TAggregate>.UnitOfWork => UnitOfWork;

        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public abstract Task<TAggregate> GetByIdAsync(int id);
        public abstract Task<IEnumerable<TAggregate>> GetAllAsync();
        public abstract Task<IEnumerable<TAggregate>> FindAsync(Expression<Func<TAggregate, bool>> predicate);
        public abstract Task<TAggregate> SingleOrDefaultAsync(Expression<Func<TAggregate, bool>> predicate);
        public abstract Task AddAsync(TAggregate aggregate);
        public abstract Task UpdateAsync(TAggregate aggregate);
        public abstract Task DeleteAsync(TAggregate aggregate);
        public abstract Task<bool> ExistsAsync(Expression<Func<TAggregate, bool>> predicate);
        public abstract Task<int> CountAsync(Expression<Func<TAggregate, bool>> predicate = null);

        protected virtual void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
                throw new BusinessRuleValidationException(rule);
        }
    }

    public interface IBusinessRule
    {
        bool IsBroken();
        string Message { get; }
        string Code { get; }
    }

    public abstract class BusinessRule : IBusinessRule
    {
        public abstract bool IsBroken();
        public abstract string Message { get; }
        public virtual string Code => GetType().Name.ToUpper();
    }

    public class BusinessRuleValidationException : DomainException
    {
        public IBusinessRule BrokenRule { get; }

        public BusinessRuleValidationException(IBusinessRule brokenRule)
            : base(brokenRule.Message, "BUSINESS_RULE_VIOLATION", brokenRule.Code)
        {
            BrokenRule = brokenRule;
        }
    }

    // Specification pattern for complex queries
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }

    public abstract class Specification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected virtual void SetCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
    }

    // Generic repository extensions
    public static class RepositoryExtensions
    {
        public static async Task<TAggregate> GetBySpecAsync<TAggregate>(
            this IRepository<TAggregate> repository,
            ISpecification<TAggregate> spec)
            where TAggregate : IAggregateRoot
        {
            var query = await repository.FindAsync(spec.Criteria);

            // Apply includes, ordering, paging as needed
            // This would be implemented based on the underlying data access technology

            return query.FirstOrDefault();
        }

        public static async Task<IEnumerable<TAggregate>> ListBySpecAsync<TAggregate>(
            this IRepository<TAggregate> repository,
            ISpecification<TAggregate> spec)
            where TAggregate : IAggregateRoot
        {
            var query = await repository.FindAsync(spec.Criteria);

            // Apply includes, ordering, paging as needed
            // This would be implemented based on the underlying data access technology

            return query;
        }
    }
}