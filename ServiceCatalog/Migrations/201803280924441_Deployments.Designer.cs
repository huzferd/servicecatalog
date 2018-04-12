// -----------------------------------------------------------------------
// <copyright file="201803280924441_Deployments.Designer.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

// <auto-generated />
namespace ServiceCatalog.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.2.0-61023")]
    public sealed partial class Deployments : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(Deployments));
        
        string IMigrationMetadata.Id
        {
            get { return "201803280924441_Deployments"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}