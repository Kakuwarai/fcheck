using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace fcheck.Models
{
    public partial class COREDBContext : DbContext
    {
        public COREDBContext()
        {
        }

        public COREDBContext(DbContextOptions<COREDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CoreVEmployeeDetail> CoreVEmployeeDetails { get; set; } = null!;
        public virtual DbSet<EvtEvent> EvtEvents { get; set; } = null!;
        public virtual DbSet<EvtEventschedule> EvtEventschedules { get; set; } = null!;
        public virtual DbSet<EvtEventschedulelog> EvtEventschedulelogs { get; set; } = null!;
        public virtual DbSet<EvtEventschedulemember> EvtEventschedulemembers { get; set; } = null!;
        public virtual DbSet<Pbcatcol> Pbcatcols { get; set; } = null!;
        public virtual DbSet<Pbcatedt> Pbcatedts { get; set; } = null!;
        public virtual DbSet<Pbcatfmt> Pbcatfmts { get; set; } = null!;
        public virtual DbSet<Pbcattbl> Pbcattbls { get; set; } = null!;
        public virtual DbSet<Pbcatvld> Pbcatvlds { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AI");

            modelBuilder.Entity<CoreVEmployeeDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("core_vEmployeeDetails");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasColumnName("birthday");

                entity.Property(e => e.Branch)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("branch");

                entity.Property(e => e.Brancharea)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("brancharea");

                entity.Property(e => e.Branchname)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("branchname");

                entity.Property(e => e.Civilstatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("civilstatus");

                entity.Property(e => e.Class)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("class");

                entity.Property(e => e.Corporate)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("corporate");

                entity.Property(e => e.CorporateName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("corporateName");

                entity.Property(e => e.Datehired)
                    .HasColumnType("datetime")
                    .HasColumnName("datehired");

                entity.Property(e => e.Department)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("department");

                entity.Property(e => e.Departmentname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("departmentname");

                entity.Property(e => e.Districtcode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("districtcode");

                entity.Property(e => e.Districtname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("districtname");

                entity.Property(e => e.Effectivitydate)
                    .HasColumnType("datetime")
                    .HasColumnName("effectivitydate");

                entity.Property(e => e.Emailadd)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("emailadd");

                entity.Property(e => e.EmplId)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("empl_id");

                entity.Property(e => e.Employeehomeaddress)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("employeehomeaddress");

                entity.Property(e => e.Employeename)
                    .HasMaxLength(153)
                    .IsUnicode(false)
                    .HasColumnName("employeename");

                entity.Property(e => e.Employeename2)
                    .HasMaxLength(108)
                    .IsUnicode(false)
                    .HasColumnName("employeename2");

                entity.Property(e => e.Employeepresentaddress)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("employeepresentaddress");

                entity.Property(e => e.Employeepresentcontact)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("employeepresentcontact");

                entity.Property(e => e.Fname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("fname");

                entity.Property(e => e.Homecontact)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("homecontact");

                entity.Property(e => e.ImmediateBday)
                    .HasColumnType("datetime")
                    .HasColumnName("ImmediateBDay");

                entity.Property(e => e.ImmediateBranch)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediateDepartment)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediateDhired)
                    .HasColumnType("datetime")
                    .HasColumnName("ImmediateDHired");

                entity.Property(e => e.ImmediateEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediateId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ImmediateID");

                entity.Property(e => e.ImmediateLevel)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediateName)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediatePosition)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediateSbu)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ImmediateSBU");

                entity.Property(e => e.ImmediateSection)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImmediateStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Joblevelgroup)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("joblevelgroup");

                entity.Property(e => e.Levelcode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("levelcode");

                entity.Property(e => e.Levelname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("levelname");

                entity.Property(e => e.Lname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lname");

                entity.Property(e => e.Mname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mname");

                entity.Property(e => e.Modifiedby)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Position)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("position");

                entity.Property(e => e.Positionname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("positionname");

                entity.Property(e => e.Section)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("section");

                entity.Property(e => e.Sectionname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("sectionname");

                entity.Property(e => e.Sex)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("sex");

                entity.Property(e => e.Sss)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("sss");

                entity.Property(e => e.Status)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.Typedescription)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("typedescription");
            });

            modelBuilder.Entity<EvtEvent>(entity =>
            {
                entity.ToTable("evt_event");

                entity.Property(e => e.Alias)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedByName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedByName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EvtEventschedule>(entity =>
            {
                entity.ToTable("evt_eventschedule");

                entity.Property(e => e.CreatedByName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Details)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Facilitator)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedByName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Venue)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EvtEventschedulelog>(entity =>
            {
                entity.ToTable("evt_eventschedulelog");

                entity.Property(e => e.CreatedByUserId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EvtEventschedulemember>(entity =>
            {
                entity.ToTable("evt_eventschedulemember");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RoomNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TableNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Team)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Pbcatcol>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("pbcatcol");

                entity.HasIndex(e => new { e.PbcTnam, e.PbcOwnr, e.PbcCnam }, "pbcatc_x");

                entity.Property(e => e.PbcBmap)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbc_bmap");

                entity.Property(e => e.PbcCase).HasColumnName("pbc_case");

                entity.Property(e => e.PbcCid).HasColumnName("pbc_cid");

                entity.Property(e => e.PbcCmnt)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbc_cmnt");

                entity.Property(e => e.PbcCnam)
                    .HasMaxLength(129)
                    .IsUnicode(false)
                    .HasColumnName("pbc_cnam");

                entity.Property(e => e.PbcEdit)
                    .HasMaxLength(31)
                    .IsUnicode(false)
                    .HasColumnName("pbc_edit");

                entity.Property(e => e.PbcHdr)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbc_hdr");

                entity.Property(e => e.PbcHght).HasColumnName("pbc_hght");

                entity.Property(e => e.PbcHpos).HasColumnName("pbc_hpos");

                entity.Property(e => e.PbcInit)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbc_init");

                entity.Property(e => e.PbcJtfy).HasColumnName("pbc_jtfy");

                entity.Property(e => e.PbcLabl)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbc_labl");

                entity.Property(e => e.PbcLpos).HasColumnName("pbc_lpos");

                entity.Property(e => e.PbcMask)
                    .HasMaxLength(31)
                    .IsUnicode(false)
                    .HasColumnName("pbc_mask");

                entity.Property(e => e.PbcOwnr)
                    .HasMaxLength(129)
                    .IsUnicode(false)
                    .HasColumnName("pbc_ownr");

                entity.Property(e => e.PbcPtrn)
                    .HasMaxLength(31)
                    .IsUnicode(false)
                    .HasColumnName("pbc_ptrn");

                entity.Property(e => e.PbcTag)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbc_tag");

                entity.Property(e => e.PbcTid).HasColumnName("pbc_tid");

                entity.Property(e => e.PbcTnam)
                    .HasMaxLength(129)
                    .IsUnicode(false)
                    .HasColumnName("pbc_tnam");

                entity.Property(e => e.PbcWdth).HasColumnName("pbc_wdth");
            });

            modelBuilder.Entity<Pbcatedt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("pbcatedt");

                entity.HasIndex(e => new { e.PbeName, e.PbeSeqn }, "pbcate_x");

                entity.Property(e => e.PbeCntr).HasColumnName("pbe_cntr");

                entity.Property(e => e.PbeEdit)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbe_edit");

                entity.Property(e => e.PbeFlag).HasColumnName("pbe_flag");

                entity.Property(e => e.PbeName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("pbe_name");

                entity.Property(e => e.PbeSeqn).HasColumnName("pbe_seqn");

                entity.Property(e => e.PbeType).HasColumnName("pbe_type");

                entity.Property(e => e.PbeWork)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("pbe_work");
            });

            modelBuilder.Entity<Pbcatfmt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("pbcatfmt");

                entity.HasIndex(e => e.PbfName, "pbcatf_x");

                entity.Property(e => e.PbfCntr).HasColumnName("pbf_cntr");

                entity.Property(e => e.PbfFrmt)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbf_frmt");

                entity.Property(e => e.PbfName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("pbf_name");

                entity.Property(e => e.PbfType).HasColumnName("pbf_type");
            });

            modelBuilder.Entity<Pbcattbl>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("pbcattbl");

                entity.HasIndex(e => new { e.PbtTnam, e.PbtOwnr }, "pbcatt_x");

                entity.Property(e => e.PbdFchr).HasColumnName("pbd_fchr");

                entity.Property(e => e.PbdFfce)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("pbd_ffce");

                entity.Property(e => e.PbdFhgt).HasColumnName("pbd_fhgt");

                entity.Property(e => e.PbdFitl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbd_fitl");

                entity.Property(e => e.PbdFptc).HasColumnName("pbd_fptc");

                entity.Property(e => e.PbdFunl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbd_funl");

                entity.Property(e => e.PbdFwgt).HasColumnName("pbd_fwgt");

                entity.Property(e => e.PbhFchr).HasColumnName("pbh_fchr");

                entity.Property(e => e.PbhFfce)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("pbh_ffce");

                entity.Property(e => e.PbhFhgt).HasColumnName("pbh_fhgt");

                entity.Property(e => e.PbhFitl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbh_fitl");

                entity.Property(e => e.PbhFptc).HasColumnName("pbh_fptc");

                entity.Property(e => e.PbhFunl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbh_funl");

                entity.Property(e => e.PbhFwgt).HasColumnName("pbh_fwgt");

                entity.Property(e => e.PblFchr).HasColumnName("pbl_fchr");

                entity.Property(e => e.PblFfce)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("pbl_ffce");

                entity.Property(e => e.PblFhgt).HasColumnName("pbl_fhgt");

                entity.Property(e => e.PblFitl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbl_fitl");

                entity.Property(e => e.PblFptc).HasColumnName("pbl_fptc");

                entity.Property(e => e.PblFunl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("pbl_funl");

                entity.Property(e => e.PblFwgt).HasColumnName("pbl_fwgt");

                entity.Property(e => e.PbtCmnt)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbt_cmnt");

                entity.Property(e => e.PbtOwnr)
                    .HasMaxLength(129)
                    .IsUnicode(false)
                    .HasColumnName("pbt_ownr");

                entity.Property(e => e.PbtTid).HasColumnName("pbt_tid");

                entity.Property(e => e.PbtTnam)
                    .HasMaxLength(129)
                    .IsUnicode(false)
                    .HasColumnName("pbt_tnam");
            });

            modelBuilder.Entity<Pbcatvld>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("pbcatvld");

                entity.HasIndex(e => e.PbvName, "pbcatv_x");

                entity.Property(e => e.PbvCntr).HasColumnName("pbv_cntr");

                entity.Property(e => e.PbvMsg)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbv_msg");

                entity.Property(e => e.PbvName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("pbv_name");

                entity.Property(e => e.PbvType).HasColumnName("pbv_type");

                entity.Property(e => e.PbvVald)
                    .HasMaxLength(254)
                    .IsUnicode(false)
                    .HasColumnName("pbv_vald");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
