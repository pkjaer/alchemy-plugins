using System;

namespace Alchemy.Plugins.Peek.Controllers
{
    public class PeekResults
    {
        public LinkResult LockedBy { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }                     // Publication and Keyword

        public string XmlName { get; set; }                 // Category
        public string UseForIdentification { get; set; }    // Category
        public string IsAbstract { get; set; }              // Keyword

        public string PublicationPath { get; set; }         // Publication
        public string PublicationUrl { get; set; }          // Publication
        public string MultimediaPath { get; set; }          // Publication
        public string MultimediaUrl { get; set; }           // Publication

        public string Directory { get; set; }               // Structure Group
        public string FullDirectory { get; set; }           // Structure Group
        public string Publishable { get; set; }             // Structure Group

        public string Namespace { get; set; }               // Schema
        public string RootElementName { get; set; }         // Schema
        public string FieldsSummary { get; set; }           // Schema
        public string MetadataFieldsSummary { get; set; }   // Schema

        public string Status { get; set; }                  // User
        public string IsAdministrator { get; set; }         // User
        public string Language { get; set; }                // User
        public string Locale { get; set; }                  // User
        public string GroupMemberships { get; set; }        // User / Group
        public string Scope { get; set; }                   // Group

        public LinkResult Schema { get; set; }
        public LinkResult LinkedSchema { get; set; }
        public LinkResult ParametersSchema { get; set; }    // Template Building Block
        public LinkResult MetadataSchema { get; set; }
        public LinkResult Template { get; set; }

        public string TemplateType { get; set; }            // Templates
        public string DynamicTemplateInfo { get; set; }     // Templates
        public string Priority { get; set; }                // Component Template

        public string Extension { get; set; }               // Page Template
        public string FileName { get; set; }                // Page / Multimedia Component
        public string FileSize { get; set; }                // Multimedia Component
        public string ComponentPresentations { get; set; }  // Page
        public string Conditions { get; set; }              // Target Group
        public string MimeType { get; set; }                // Multimedia Type
        public string FileExtensions { get; set; }          // Multimedia Type
        public string DefaultCodePage { get; set; }         // Publication Target
        public string TargetLanguage { get; set; }          // Publication Target
        public string Destinations { get; set; }            // Publication Target

        public int? Versions { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Creator { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string Revisor { get; set; }
        
        public string PublishPath { get; set; }
        public string WebDavUrl { get; set; }
    }
}