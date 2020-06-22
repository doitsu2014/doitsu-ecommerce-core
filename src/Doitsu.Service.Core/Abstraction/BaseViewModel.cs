using AutoMapper;
using System;

namespace Doitsu.Service.Core.Abstraction
{
    public class BaseViewModel<TEntity> where TEntity : class, new()
    {
        private IMapper _mapper;
        public BaseViewModel()
        {
            this.SelfType = base.GetType();
            this.EntityType = typeof(TEntity);
        }

        public BaseViewModel(TEntity entity, IMapper mapper)
            :this()
        {
            _mapper = mapper;
            this.CopyFromEntity(entity);
        }

        public void SetMapper (IMapper mapper){  _mapper = mapper;  }

        protected Type SelfType { get; set; }
        protected Type EntityType { get; set; }
        public void CopyFromEntity(TEntity entity)
        {
            _mapper.Map((object)entity, (object)this, this.EntityType, this.SelfType);
        }
        public void CopyFromEntity(TEntity entity, IMapper mapper)
        {
            mapper.Map((object)entity, (object)this, this.EntityType, this.SelfType);
        }
        public void CopyToEntity(TEntity entity)
        {
            _mapper.Map((object)this, (object)entity, this.SelfType, this.EntityType);
        }
        public void CopyToEntity(TEntity entity, IMapper mapper)
        {
            mapper.Map((object)this, (object)entity, this.SelfType, this.EntityType);
        }
        public TEntity ToEntity()
        {
            TEntity rs = new TEntity();
            this.CopyToEntity(rs);
            return rs;
        }
        public TEntity ToEntity(IMapper mapper)
        {
            TEntity rs = new TEntity();
            this.CopyToEntity(rs, mapper);
            return rs;
        }
    }
}