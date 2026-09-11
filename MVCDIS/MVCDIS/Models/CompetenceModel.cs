using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace MVCDIS.Models
{






    public class CompetenceContext : DbContext
    {

        public CompetenceContext()
            : base("DefaultConnection")
        {
        }
        public DbRawSqlQuery<ListPath> ListTreePath(int ACT)
        {
            var sql = @"ListTreePath {0}";
            return Database.SqlQuery<ListPath>(sql, ACT);
        }
        public DbSet<DisciplineClient> _DisciplineClient { get; set; }

        public DbSet<Levels> _Levels { get; set; }
        public DbSet<ClientsCards> _ClientsCards { get; set; }
        public DbSet<Attributes> _Attributes { get; set; }
        public DbSet<Competence> _Competence { get; set; }
        public DbSet<Certification> _Certification { get; set; }
        public DbSet<JobTitle> _JobTitle { get; set; }
        public DbSet<JobTitleCo> _JobTitleCo { get; set; }
        public DbSet<JobTitleAt> _JobTitleAt { get; set; }
        public DbSet<ClientsInfo> _ClientsInfo { get; set; }

        public DbSet<Clients> _Clients { get; set; }
        public DbSet<ValueCompetence> _ValueCompetence { get; set; }
        public DbSet<CalcValueCompetence> _CalcValueCompetence { get; set; }
        public DbSet<TypeAttributes> _TypeAttributes { get; set; }

        public DbSet<ProfArea> _ProfArea { get; set; }

        public DbSet<Plans> _Plans { get; set; }

        public DbSet<iasaWorks> tableIasaWorks { get; set; }
        public DbSet<iasaActivities> tableIasaActivities { get; set; }
        public DbSet<iasaWorkByUserActivity> tableIasaWorkByUserActivity { get; set; }
        public DbSet<DisciplineAttributes> _DisciplineAttributes { get; set; }
        public DbSet<iasaStepen> tableIasaStepen { get; set; }
        public DbSet<iasaZvanie> tableIasaZvanie { get; set; }
        public DbSet<iasaUserActivity> tableIasaUserActivity { get; set; }
        public DbSet<iasaUserWork> tableIasaUserWork { get; set; }
        public DbSet<iasaWorktypes> tableIasaWorktypes { get; set; }
        public DbSet<iasaTemplates> tableIasaTemplates { get; set; }
        public DbSet<iasaWorktypesTree> tableIasaWorktypesTree { get; set; }
        public DbSet<iasaReports> tableIasaReports { get; set; }
        public DbSet<Disciplines> _Disciplines { get; set; }
        public DbSet<EventsAttributes> _EventsAttributes { get; set; }
         public DbSet<iasaIspondArchive> _iasaIspondArchive { get; set; }
         public DbSet<ReportsList> _ReportsList { get; set; }
    

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Levels>().
              HasMany(c => c.CompetenceAttending).
              WithMany(p => p.LevelsAttending).
              Map(
               m =>
               {
                   m.MapLeftKey("LevelID");
                   m.MapRightKey("CompetenceID");
                   m.ToTable("Cert");
               });

        }

        public System.Data.Entity.DbSet<MVCDIS.Models.CompetenceEdit> CompetenceEdits { get; set; }
        public System.Data.Entity.DbSet<MVCDIS.Models.MarksEdit> MarksEdits { get; set; }
        public System.Data.Entity.DbSet<MVCDIS.Models.ClientCalcValueEdit> ClientCalcValueEdits { get; set; }

        public System.Data.Entity.DbSet<MVCDIS.Models.Roles> Roles { get; set; }

    }
    public class Listofvar
    {
        public int DisciplineID { get; set; }
        public string Discipline { get; set; }
        public int Mark { get; set; }
        public int CompetenceID { get; set; }
        public double Value { get; set; }
        public int Time { get; set; }
    }

    public class DiscipSort
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string Source { get; set; }
        public double Rstart { get; set; }
        public double Rend { get; set; }
        public double ft { get; set; }
        public int Time { get; set; }
    }
    public class DisciplineViewModel
    {
        public IEnumerable<string> SelectedDisciplines { get; set; }
        public IEnumerable<SelectListItem> Disciplines { get; set; }
    }
    public class DisciplineEdit
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        [Required(ErrorMessage = "Оценка не может быть пустой")]

        [Range(0, 5, ErrorMessage = "Неверный диапазон оценки")]
        public int? Mark { get; set; }

    }
    public class ClientCalcValueEdit
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int ClientID { get; set; }

        public string ClientName { get; set; }


        public double value { get; set; }

    }


    [Table("DisciplineClient")]
    public class DisciplineClient
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? ClientID { get; set; }
        public int DisciplineID { get; set; }
        public int? Mark { get; set; }
        [ForeignKey("ClientID")]
        public virtual Clients Clients { get; set; }
        [ForeignKey("DisciplineID")]
        public virtual Disciplines Disciplines { get; set; }
    }

    [Table("DisciplineAttributes")]
    public class DisciplineAttributes
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int AttributesID { get; set; }
        public int DisciplineID { get; set; }
        [ForeignKey("AttributesID")]
        public virtual Attributes Attributes { get; set; }
        [ForeignKey("DisciplineID")]
        public virtual Disciplines Disciplines { get; set; }
    }


    [Table("EventsAttributes")]
    public class EventsAttributes
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int AttributesID { get; set; }
        public int ActivityId { get; set; }
        public int WorktypeId { get; set; }
        [ForeignKey("AttributesID")]
        public virtual Attributes Attributes { get; set; }
        [ForeignKey("WorktypeId")]
        public virtual iasaWorktypesTree iasaWorktypesTree { get; set; }
        [ForeignKey("ActivityId")]
        public virtual iasaActivities iasaActivities { get; set; }
    }
    public class EventsAttributesNEW
    {

        public int ID { get; set; }

        public int AttributesID { get; set; }
        public int ActivityId { get; set; }
        public int WorktypeId { get; set; }
        public string Name { get; set; }
    }
    [Table("Disciplines")]
    public class Disciplines
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [DisplayName("Название материала")]
        public string Discipline { get; set; }
        [Required]
        [DisplayName("Время обучения часы")]
        public int Time { get; set; }
        [Required]
        [DisplayName("Источник материала")]
        public string Source { get; set; }
    }
    public class selgrp
    {
        public int ID { get; set; }
        public int mark { get; set; }
        public int max { get; set; }
        public int min { get; set; }
        public string Lev { get; set; }

        public string Name { get; set; }
        public DateTime Date { get; set; }

    }
    [Table("Plans")]
    public class Plans
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BeginCompetence { get; set; }
        public int ClientID { get; set; }
        public int LevelID { get; set; }
        public int JobID { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public int mark { get; set; }

        [ForeignKey("BeginCompetence")]
        public virtual Competence Competence { get; set; }
        [ForeignKey("ClientID")]
        public virtual Clients Clients { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }
        [ForeignKey("JobID")]
        public virtual JobTitle JobTitle { get; set; }

    }




    public class CalcResult
    {

        public int CompetenceID { get; set; }
        public int AttributesID { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Grade { get; set; }

        public double CalcCol { get; set; }

    }
    public class CalcGroupCOResult
    {

        public string Shifr { get; set; }


        public double Value { get; set; }


    }
    [Table("CalcCo")]
    public class CalcCo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int JobTitleID { get; set; }

        public int LevelID { get; set; }

        public int CompetenceID { get; set; }

        public double _MiN { get; set; }
        public double _MAX { get; set; }
        public int MinClientID { get; set; }
        public int MaxClientID { get; set; }

    }

    public class RANGEVALUES
    {

        public string Shifr { get; set; }

        public string Name { get; set; }
        public int CompetenceID { get; set; }

        public double _MiN { get; set; }
        public double _MAX { get; set; }
        public int MinClientID { get; set; }
        public string MinClientName { get; set; }
        public int MaxClientID { get; set; }
        public string MaxClientName { get; set; }

    }

    [Table("ValueCompetence")]
    public class ValueCompetence
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }


        [Required]
        public int ClientID { get; set; }


        [Required]
        public int CompetenceID { get; set; }
        [Required]
        public int JobTitleID { get; set; }
        [Required]
        public int LevelID { get; set; }
        [Required]
        public double value { get; set; }

        [ForeignKey("JobTitleID")]
        public virtual JobTitle JobTitle { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }
        [ForeignKey("CompetenceID")]
        public virtual Competence Competences { get; set; }
        [ForeignKey("ClientID")]
        public virtual Clients Clients { get; set; }
    }

    public class Calculate
    {
        public double value { get; set; }
    }
    [Table("iasaActivities")]
    public class iasaActivities
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }
        [Display(Name = "Виды мероприятия")]
        public string ActivityName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата начала")]
        public DateTime? BeginDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата окончания")]
        public DateTime? EndDate { get; set; }
    }
    [Table("iasaUserActivity")]
    public class iasaUserActivity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public int DefaultWorkId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile iasaUser { get; set; }

        [ForeignKey("ActivityId")]
        public virtual iasaActivities iasaActivities { get; set; }
    }

    [Table("iasaStepen")]
    public class iasaStepen
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StepenId { get; set; }
        [Display(Name = "Степень")]
        public string StepenName { get; set; }
    }

    [Table("iasaWorks")]
    public class iasaWorks
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WorkId { get; set; }
        [Display(Name = "Вид занятости")]
        public string WorkName { get; set; }

    }
    [Table("iasaZvanie")]
    public class iasaZvanie
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ZvanieId { get; set; }
        [Display(Name = "Звание")]
        public string ZvanieName { get; set; }
    }

    [Table("iasaReports")]
    public class iasaReports
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public int WorktypeId { get; set; }
        public string ReportText { get; set; }
        public DateTime ReportBegin { get; set; }
        public DateTime ReportEnd { get; set; }

        public int WorkId { get; set; }
        public int ActivityId { get; set; }
        [Range(0, 5, ErrorMessage = "Неверный диапазон оценки")]
        public int? Grade { get; set; }
        public DateTime ReportCreate { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile iasaUser { get; set; }

        [ForeignKey("WorktypeId")]
        public virtual iasaWorktypesTree iasaWorktypesTree { get; set; }

        [ForeignKey("WorkId")]
        public virtual iasaWorks iasaWorks { get; set; }

        [ForeignKey("ActivityId")]
        public virtual iasaActivities iasaActivities { get; set; }
    }
    [Table("iasaWorkByUserActivity")]
    public class iasaWorkByUserActivity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int WorkId { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("WorkId")]
        public virtual iasaWorks iasaWorks { get; set; }
        [ForeignKey("ActivityId")]
        public virtual iasaActivities iasaActivities { get; set; }




    }

    [Table("iasaWorktypesTree")]
    public class iasaWorktypesTree
    {
        public iasaWorktypesTree()
        {
            IsDeleted = false;
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WorktypeId { get; set; }
        public int? ParentId { get; set; }
      
      
        public string WorktypeName { get; set; }
        public int ActivityId { get; set; }
        public bool IsDeleted { get; set; }
        public int sort { get; set; }
        [Range(0, 5, ErrorMessage = "Неверный диапазон оценки")]
        public int? MinGrade { get; set; }
        [Range(0, 5, ErrorMessage = "Неверный диапазон оценки")]
        public int? MaxGrade { get; set; }

        [ForeignKey("ActivityId")]
        public virtual iasaActivities iasaActivity { get; set; }

    }
    [Table("iasaTemplates")]
    public class iasaTemplates
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TemplateId { get; set; }
        public int UserId { get; set; }
        public byte[] TemplateData { get; set; }
        public string TemplateName { get; set; }
        public string TemplateMime { get; set; }
        public int TemplateSize { get; set; }
        public int TemplateActivity { get; set; }
        public bool TemplateIsDefault { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile iasaUser { get; set; }
        [ForeignKey("TemplateActivity")]
        public virtual iasaActivities iasaACtivities { get; set; }
    }
    [Table("iasaUserWork")]
    public class iasaUserWork
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public int WorkId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfile iasaUser { get; set; }

        [ForeignKey("WorkId")]
        public virtual iasaWorks iasaWorks { get; set; }
    }
    [Table("iasaIspondArchive")]
    public class iasaIspondArchive
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int archive_id { get; set; }
        public int archive_user_id { get; set; }
        [ForeignKey("archive_user_id")]
        public virtual UserProfile UserProfile { get; set; }

        public int archive_work_id { get; set; }
        [ForeignKey("archive_work_id")]
        public virtual iasaWorks iasaWorks { get; set; }
        public int archive_activity_id { get; set; }
        [ForeignKey("archive_activity_id")]
        public virtual iasaActivities iasaActivities { get; set; }
        public DateTime archive_year_begin { get; set; }
        public DateTime archive_year_end { get; set; }
        public DateTime archive_year_create { get; set; }
        public String archive_comments { get; set; }
        public int archive_grade { get; set; }
        public Int64 archive_group { get; set; }
        public int archive_JobId { get; set; }
        [ForeignKey("archive_JobId")]
        public virtual JobTitle JobTitle { get; set; }
        public int archive_worktype_id { get; set; }
        [ForeignKey("archive_worktype_id")]
        public virtual iasaWorktypesTree iasaWorktypesTree { get; set; }
    }


    [Table("ReportsList")]
    public class ReportsList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Int64 GroupId { get; set; }
      
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        public int ActivityId { get; set; }
        [ForeignKey("ActivityId")]
        public virtual iasaActivities iasaActivities { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual JobTitle JobTitle { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateCreate { get; set; }
        public int WorkId { get; set; }
        [ForeignKey("WorkId")]
        public virtual iasaWorks iasaWorks { get; set; }
        public bool Confirmat { get; set; }
      
    }




    [Table("iasaWorktypes")]
    public class iasaWorktypes
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WorktypeId { get; set; }
        [Display(Name = "Мероприятие")]
        public string WorktypeName { get; set; }
    }

    [Table("CalcValueCompetence")]
    public class CalcValueCompetence
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }


        [Required]
        public int ClientID { get; set; }



        [Required]
        public int JobTitleID { get; set; }
        [Required]
        public int LevelID { get; set; }
        [Required]
        public double value { get; set; }

        [ForeignKey("JobTitleID")]
        public virtual JobTitle JobTitle { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }

        [ForeignKey("ClientID")]
        public virtual Clients Clients { get; set; }
    }



    [Table("ClientsInfo")]
    public class ClientsInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public String FIO { get; set; }
        [Required]
        public String Email { get; set; }

    }

    public class Templates
    {
        public List<iasaTemplates> userTemplates { get; set; }
        public int defaultTemplate { get; set; }
    }

    [Table("Clients")]
    [TableName("Перечень кандидатов")]
    public class Clients
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public bool Manager { get; set; }


        [Required]
        [DisplayName("Должность кандидата")]
        public int JobTitleID { get; set; }
        [Required]
        [DisplayName("ФИО кандидата")]
        public int ClientInfoID { get; set; }
        [Required]
        [DisplayName("Степень")]
        public int StepenId { get; set; }
      

        public int? DefaultActivityId { get; set; }
        [Required]
        [DisplayName("Звание")]
        public int ZvanieId { get; set; }
        [ForeignKey("JobTitleID")]

        public virtual JobTitle JobTitle { get; set; }
        [ForeignKey("ClientInfoID")]

        public virtual ClientsInfo ClientsInfo { get; set; }

        [ForeignKey("StepenId")]

        public virtual iasaStepen iasaStepen { get; set; }

        [ForeignKey("ZvanieId")]

        public virtual iasaZvanie iasaZvanie { get; set; }
        [ForeignKey("DefaultActivityId")]

        public virtual iasaActivities iasaActivities { get; set; }



    }



    [Table("JobTitleCo")]
    public class JobTitleCo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int JobTitleID { get; set; }
        [Required]
        public int CompetenceID { get; set; }
        [Required]
        public int LevelID { get; set; }
        [Required]
        public double Value { get; set; }
        [ForeignKey("JobTitleID")]
        public virtual JobTitle JobTitles { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }
        [ForeignKey("CompetenceID")]
        public virtual Competence Competences { get; set; }
    }


    [Table("JobTitleAt")]
    public class JobTitleAt
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int JobTitleID { get; set; }
        [Required]
        public int AttributesID { get; set; }
        [Required]
        public int CompetenceID { get; set; }
        [Required]
        public int LevelID { get; set; }
        [Required]
        public double Value { get; set; }
        [ForeignKey("AttributesID")]
        public virtual Attributes Attributes { get; set; }
        [ForeignKey("JobTitleID")]
        public virtual JobTitle JobTitles { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }
        [ForeignKey("CompetenceID")]
        public virtual Competence Competences { get; set; }
    }



    [Table("JobTitle")]
    public class JobTitle
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int ProfAreaID { get; set; }

        [ForeignKey("ProfAreaID")]
        public virtual ProfArea ProfAreas { get; set; }
        [Required]
        public String Name { get; set; }

    }


    [Table("ProfArea")]
    public class ProfArea
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public String Name { get; set; }
    }


    [Table("ClientsCards")]
    public class ClientsCards
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int? ClientID { get; set; }
        [Required]
        public int AttributeID { get; set; }
        [Required]
        public int LevelID { get; set; }
        [Required]
        public double Grade { get; set; }
        [ForeignKey("ClientID")]
        public virtual Clients Clients { get; set; }
        [ForeignKey("AttributeID")]
        public virtual Attributes Attributes { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }


    }
    [Table("TypeAttributes")]
    public class TypeAttributes
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [DisplayName("Название")]
        public String Name { get; set; }
    }

    [Table("Attributes")]
    public class Attributes
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [DisplayName("Шифр")]
        public int CompetenceID { get; set; }
        [ForeignKey("CompetenceID")]
        public virtual Competence Competence { get; set; }
        [Required]
        [DisplayName("Название")]
        public String Name { get; set; }
        [Required]
        public int TypeID { get; set; }
        [ForeignKey("TypeID")]
        public virtual TypeAttributes TypeAttributes { get; set; }
    }
    [Table("Levels")]
    [TableName("Уровни компетентности")]
    public class Levels
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [DisplayName("Уровень")]
        public string Lev { get; set; }
        [Required]
        [DisplayName("Название")]
        public String Name { get; set; }




        public ICollection<Competence> CompetenceAttending { get; set; }
    }
    [Table("Competence")]
    [TableName("Компетенции")]
    public class Competence
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]

        public int LevelID { get; set; }
        [DisplayName("Уровень")]
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }

        [Required]
        [DisplayName("Шифр")]
        public String Shifr { get; set; }
        [DisplayName("Название")]
        [Required]
        public String Name { get; set; }


        [ForeignKey("LevelID")]
        public ICollection<Levels> LevelsAttending { get; set; }
    }

    public class CompetenceEdit
    {

        public int ID { get; set; }
        public String Shifr { get; set; }
        public String Name { get; set; }

        public string Value { get; set; }
    }
    public class AttributesEdit
    {

        public int ID { get; set; }
        public String Name { get; set; }
        [Range(0, 100)]
        public string Value { get; set; }
    }
    public class MarksEdit
    {

        public int ID { get; set; }
        public String Name { get; set; }
        [Range(0, 5)]
        public double Grade { get; set; }
    }
    public class QuizEdit
    {

        public int ID { get; set; }
        public String Name { get; set; }

    }
    public class AttributesEditViewModel
    {
        public List<AttributesEdit> AttributesEdits { get; set; }
    }
    public class QuizEditViewModel
    {
        public List<QuizEdit> QuizEdits { get; set; }
    }
    public class MarksEditViewModel
    {
        public List<MarksEdit> MarksEdits { get; set; }
    }
    public class CompetenceEditViewModel
    {
        public List<CompetenceEdit> CompetenceEdits { get; set; }
    }
    public class ClientCalcValueEditViewModel
    {
        public List<ClientCalcValueEdit> ClientCalcValueEdits { get; set; }
    }
    [Table("Certification")]
    public partial class Certification
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]

        public int LevelID { get; set; }
        [ForeignKey("LevelID")]
        public virtual Levels Levels { get; set; }

        [Required]
        [DisplayName("Шифр")]
        public int CompetenceID { get; set; }
        [ForeignKey("CompetenceID")]
        public virtual Competence Competence { get; set; }
    }


}