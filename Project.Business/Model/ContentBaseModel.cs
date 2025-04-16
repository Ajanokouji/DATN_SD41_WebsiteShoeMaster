using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SERP.Framework.Entities;
using SERP.Framework.Entities.Enums;
using SERP.Framework.Entities.Interfaces;


namespace SERP.NewsMng.Business.Models;

public class ContentBaseModel: BaseACLEntity,  IPublishEntity, IIsPublishEntity,
    ISoftDelete,  ILabelObjectsEntity,  ITitleEntity, ITypeEntity,

	IImageEntity,  ICompletePathEntity, IOrderEntity, IStatusObjectEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public  string ImageUrl { get; set; }
    public virtual Guid? ParentId { get; set; }
    
    public List<LabelsObj> LabelsObjs { get; set; }

	public virtual  string Type { get; set; }
	public string CompletePath { get; set; }

    public string CompleteCode { get; set; }

    public string CompleteName { get; set; }
    
    public Guid? OwnerId { get;set; }
    /// <summary>
    /// The SEO uri.
    /// </summary>
    [MaxLength(500)]
    public string SeoUri { get; set; }

    /// <summary>
    /// The SEO title.
    /// </summary>
    [MaxLength(500)]
    public string SeoTitle { get; set; }

    /// <summary>
    /// The SEO description.
    /// </summary>
    [MaxLength(1000)]
    public string SeoDescription { get; set; }

	/// <summary>
	/// The SEO keywords.
	/// </summary>f
	[MaxLength(1000)]
    public string SeoKeywords { get; set; }

    /// <summary>
    /// The flag which defines the file is published or not.
    /// </summary>
    public virtual bool? IsPublish { get; set; }

    public virtual DateTime? PublishOnDate { get; set; }

    /// <summary>
    /// The publish start date.
    /// </summary>
    public virtual DateTime? PublishStartDate { get; set; }

    public virtual DateTime? PublishEndDate { get; set; }
    
    
    public Guid? PageContentId { get; set; }
    public string PageContentCode { get; set; }
    public string PageContentName { get; set; }
    public string PageContentUri { get; set; }
    
    public int? Order { get; set; }
    
    /// <summary>
    /// The flag which defines the file is searchable or not.
    /// </summary>
    public virtual bool? IsSearchable { get; set; }
    
    public virtual StatusEnum StatusObj { get; set; } 
    public virtual bool IsDeleted { get; set; }
}