using System;
using System.Web.Script.Serialization;
using Iesi.Collections.Generic;

namespace CourseSampleApp.Models
{
    public class Blog
    {
    	public virtual int Id { get; set; }

    	public virtual string Title { get; set; }

    	public virtual string Subtitle { get; set; }

    	public virtual bool AllowsComments { get; set; }

    	public virtual DateTime CreatedAt { get; set; }

		public virtual ISet<User> Users { get; set; }

    	public Blog()
    	{
    		Users = new HashedSet<User>();
    	}
    }
}