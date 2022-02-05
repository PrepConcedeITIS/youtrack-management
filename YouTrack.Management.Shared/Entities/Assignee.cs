using System;
using System.Collections.Generic;

namespace YouTrack.Management.Shared.Entities
{
    public class Assignee : HasId, ICustomFieldValue
    {
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<AssigneeCompetenceRelation> Competences { get; set; }

        public bool Banned { get; set; }

        public Assignee(string login, string fullName, string name, string id, string email) : base(id)
        {
            Login = login;
            FullName = fullName;
            Name = name;
            Email = email;
        }
    }

    public class AssigneeEqualityComparer : IEqualityComparer<Assignee>
    {
        public bool Equals(Assignee x, Assignee y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Login == y.Login && x.FullName == y.FullName && x.Name == y.Name && x.Email == y.Email && x.Banned == y.Banned;
        }

        public int GetHashCode(Assignee obj)
        {
            return HashCode.Combine(obj.Login, obj.FullName, obj.Name, obj.Email, obj.Competences, obj.Banned);
        }
    }

    public class AssigneeCompetenceRelation
    {
        public long Id { get; set; }
        public string AssigneeId { get; set; }
        public virtual Assignee Assignee { get; set; }
        public long CompetenceId { get; set; }
        public virtual Competence Competence { get; set; }
    }
}