﻿

//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Franquisia1._1
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class appbosaEntities : DbContext
{
    public appbosaEntities()
        : base("name=appbosaEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public DbSet<ciafile> ciafile { get; set; }

    public DbSet<divatencion> divatencion { get; set; }

    public DbSet<sucursal> sucursal { get; set; }

    public DbSet<undatencion> undatencion { get; set; }

    public DbSet<usrfile> usrfile { get; set; }

    public DbSet<sucusr> sucusr { get; set; }

    public DbSet<claserv> claserv { get; set; }

    public DbSet<peratencion> peratencion { get; set; }

    public DbSet<sucperatencion> sucperatencion { get; set; }

    public DbSet<conventa> conventa { get; set; }

    public DbSet<anexos> anexos { get; set; }

    public DbSet<conc> conc { get; set; }

    public DbSet<cond> cond { get; set; }

    public DbSet<punemi> punemi { get; set; }

    public DbSet<convensuc> convensuc { get; set; }

    public DbSet<preconven> preconven { get; set; }

    public DbSet<maesgen> maesgen { get; set; }

    public DbSet<rolespv> rolespv { get; set; }

    public DbSet<ciausrfile> ciausrfile { get; set; }

    public DbSet<parreg> parreg { get; set; }

    public DbSet<tdcom> tdcom { get; set; }

    public DbSet<forventa> forventa { get; set; }

    public DbSet<forpago> forpago { get; set; }

    public DbSet<tarjetas> tarjetas { get; set; }

    public DbSet<venc> venc { get; set; }

    public DbSet<numpos> numpos { get; set; }

    public DbSet<venpag> venpag { get; set; }

    public DbSet<numcia> numcia { get; set; }

    public DbSet<numpemi> numpemi { get; set; }

    public DbSet<tdprgven> tdprgven { get; set; }

    public DbSet<vend> vend { get; set; }

}

}

