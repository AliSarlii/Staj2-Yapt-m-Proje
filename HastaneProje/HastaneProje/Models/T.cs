//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HastaneProje.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T()
        {
            this.Tiroid_Verileri = new HashSet<Tiroid_Verileri>();
        }
    
        public int Id { get; set; }
        public string t_Veri { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tiroid_Verileri> Tiroid_Verileri { get; set; }
    }
}
