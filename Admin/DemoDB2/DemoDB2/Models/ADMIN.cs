//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ADMIN
    {
        public int MAQUANTRI { get; set; }
        public string TENDANGNHAP { get; set; }
        [DataType(DataType.Password)]
        public string MATKHAU { get; set; }
    }
}