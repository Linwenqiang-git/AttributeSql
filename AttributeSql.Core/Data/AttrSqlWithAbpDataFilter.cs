using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace AttributeSql.Core.Data
{
    public class AttrSqlWithAbpDataFilter : IAttrSqlWithAbpDataFilter, ITransientDependency
    {
        private ICurrentTenant? _currentTenant;
        private AbpDataFilterOptions _abpDataFilter;        
        public AttrSqlWithAbpDataFilter(IOptions<AbpDataFilterOptions> abpDataFilterOptions)
        {            
            _abpDataFilter = abpDataFilterOptions.Value;
            AddDefaultGlobalDataFilter();
        }
        public AttrSqlWithAbpDataFilter(IOptions<AbpDataFilterOptions> abpDataFilterOptions, ICurrentTenant currentTenant = null):this(abpDataFilterOptions)
        {
            _currentTenant = currentTenant;
        }
        private void AddDefaultGlobalDataFilter()
        {
            if (!_abpDataFilter.DefaultStates.ContainsKey(typeof(IMultiTenant)))
            {
                _abpDataFilter.DefaultStates.TryAdd(typeof(IMultiTenant), new DataFilterState(true));
            }
            if (!_abpDataFilter.DefaultStates.ContainsKey(typeof(ISoftDelete)))
            {
                _abpDataFilter.DefaultStates.TryAdd(typeof(ISoftDelete), new DataFilterState(true));
            }
        }
        public IDisposable Disable()
        {
            foreach (var filter in _abpDataFilter.DefaultStates)
            {
                filter.Value.IsEnabled = false;
            }
            return new DisposeAction(() => Disable());
        }
        public IDisposable Disable<T>()
        {
            _abpDataFilter.DefaultStates.TryGetValue(typeof(T),out var dataFilterState);
            if(dataFilterState != null)
                dataFilterState.IsEnabled = false;
            return new DisposeAction(() => Disable<T>());
        }
        public IDisposable Enable()
        {
            foreach (var filter in _abpDataFilter.DefaultStates)
            {
                filter.Value.IsEnabled = false;
            }
            return new DisposeAction(() => Disable());
        }
        public IDisposable Enable<T>()
        {
            _abpDataFilter.DefaultStates.TryGetValue(typeof(T), out var dataFilterState);
            if (dataFilterState != null)
                dataFilterState.IsEnabled = true;
            return new DisposeAction(() => Enable<T>());
        }

        public Guid? GetCurrentTenantId()
        {
            _abpDataFilter.DefaultStates.TryGetValue(typeof(IMultiTenant), out var dataFilterState);
            if (dataFilterState != null && dataFilterState.IsEnabled)
                return _currentTenant?.GetId();
            else
                return default;
        }

        public bool? IsFilterDelete()
        {
            _abpDataFilter.DefaultStates.TryGetValue(typeof(ISoftDelete), out var dataFilterState);            
            return dataFilterState?.IsEnabled;
        }
    }
}
