﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UAGUtileriasSAT.DataBase
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PRNFACTEntities : DbContext
    {
        public PRNFACTEntities()
            : base("name=PRNFACTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UAG_CFDI_CP_LN> UAG_CFDI_CP_LN { get; set; }
        public virtual DbSet<UAG_CFDI_HDR> UAG_CFDI_HDR { get; set; }
        public virtual DbSet<UAG_CFDI_IM_LOC> UAG_CFDI_IM_LOC { get; set; }
        public virtual DbSet<UAG_CFDI_LN> UAG_CFDI_LN { get; set; }
        public virtual DbSet<UAG_CFDI_LN_IMP> UAG_CFDI_LN_IMP { get; set; }
        public virtual DbSet<UAG_COMPAG_HDR> UAG_COMPAG_HDR { get; set; }
        public virtual DbSet<UAG_COMPAG_LN> UAG_COMPAG_LN { get; set; }
        public virtual DbSet<UAG_COMPAG_PYMNT> UAG_COMPAG_PYMNT { get; set; }
        public virtual DbSet<UAG_COMPAG_PYMNT_DOCREL> UAG_COMPAG_PYMNT_DOCREL { get; set; }
        public virtual DbSet<UAG_LOG> UAG_LOG { get; set; }
        public virtual DbSet<UAG_MESSAGES_SOLUCIONFACTIBLE> UAG_MESSAGES_SOLUCIONFACTIBLE { get; set; }
        public virtual DbSet<UAG_UUID_LOG> UAG_UUID_LOG { get; set; }
        public virtual DbSet<UAG_VENDOR> UAG_VENDOR { get; set; }
    }
}
