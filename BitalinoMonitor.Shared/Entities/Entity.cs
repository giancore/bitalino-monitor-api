using System;
using FluentValidator;

namespace BitalinoMonitor.Shared.Entities
{
    public abstract class Entity : Notifiable
    {
        public Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}