using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Runtime.InteropServices;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Infrastructure;
using System.Configuration;
using WisDot.Bos.Sct.Core.Data.Interfaces;

namespace WisDot.Bos.Sct.Core.Data
{
    public class ReportWriterQuery : IReportWriterQuery
    {
        private static string bosCdPdfFilePath = ConfigurationManager.AppSettings["bosCdPdfFilePath"].ToString();
        private static string signatureFilePath = ConfigurationManager.AppSettings["signatureFilePath"].ToString();
        private static string tempDir = ConfigurationManager.AppSettings["tempDir"].ToString();

        public void CreateCdPdfFile(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath)
        {
            FileStream fs = new FileStream(xfdfFilePath, FileMode.Open);
            //XfdfReader xfdfReader = new XfdfReader(new FileStream(xfdfFilePath, FileMode.Open));
            XfdfReader xfdfReader = new XfdfReader(fs);
            PdfReader pdfReader = new PdfReader(pdfFilePath);
            FileStream fileStream = new FileStream(newPdfFilePath, FileMode.Create);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, fileStream);
            pdfStamper.AcroFields.SetFields(xfdfReader);

            if (File.Exists(signatureFilePath))
            {
                try
                {
                    PushbuttonField signature = pdfStamper.AcroFields.GetNewPushbuttonFromField("DesignChiefSignature");
                    signature.Layout = PushbuttonField.LAYOUT_ICON_ONLY;
                    signature.ProportionalIcon = true;
                    signature.Image = iTextSharp.text.Image.GetInstance(signatureFilePath);
                    pdfStamper.AcroFields.ReplacePushbuttonField("DesignChiefSignature", signature.Field);
                }
                catch (Exception ex)
                { }
            }

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader.Close();
            fs.Close();

            try
            {
                File.Delete(xfdfFilePath);
            }
            catch { }
        }

        public void CreateCdXpdfFile(string xfdfFilePath, string newPdfFilePath, Project project, Structure structure, WorkConcept workConcept,
                        DateTime dateTime, Project fiipsProject, WorkConcept certificationWorkConcept)
        {
            string designId = "";
            string constructionId = project.FosProjectId;
            string epseDate = "";
            string pseDate = "";
            string letDate = "";
            string epseEarliest = project.AcceptablePseDateStart.ToString("MM/dd/yyyy");
            string epseLatest = project.AcceptablePseDateEnd.ToString("MM/dd/yyyy");
            string letEarliestFy = "";
            string letLatestFy = "";


            if (fiipsProject != null)
            {
                try
                {
                    designId = Utility.FormatConstructionId(fiipsProject.DesignId);
                }
                catch { }

                try
                {
                    constructionId = Utility.FormatConstructionId(fiipsProject.FosProjectId);
                }
                catch { }

                try
                {
                    epseDate = fiipsProject.EpseDate.Year != 1 ? fiipsProject.EpseDate.ToString("MM/dd/yyyy") : "";
                }
                catch { }

                try
                {
                    pseDate = fiipsProject.PseDate.Year != 1 ? fiipsProject.PseDate.ToString("MM/dd/yyyy") : "";
                }
                catch { }

                try
                {
                    letDate = fiipsProject.FiscalYear.ToString();
                }
                catch { }

                DateTime epseStart;

                if (fiipsProject.EpseDate.Year != 1)
                {
                    epseStart = fiipsProject.EpseDate;
                }
                else
                {
                    epseStart = fiipsProject.PseDate;
                }

                DateTime epseEnd = epseStart.AddYears(3);
                DateTime letStart = epseStart.AddMonths(3);
                DateTime letEnd = letStart.AddYears(3);
                //epseEarliest = project.AdvanceableFiscalYear != 0 ? new DateTime(project.AdvanceableFiscalYear - 1, 7, 1).ToString("yyyy/MM/dd") : new DateTime(project.FiscalYear - 1, 7, 1).ToString("yyyy/MM/dd");
                //epseLatest = new DateTime(project.FiscalYear, 6, 30).ToString("yyyy/MM/dd");

                if (letStart.Month >= 7)
                {
                    letEarliestFy = (letStart.Year + 1).ToString();
                }
                else
                {
                    letEarliestFy = letStart.Year.ToString();
                }

                if (letEnd.Month >= 7)
                {
                    letLatestFy = (letEnd.Year + 1).ToString();
                }
                else
                {
                    letLatestFy = letEnd.Year.ToString();
                }
            }

            string certificationDate = "";
            string certificationLiaison = certificationWorkConcept.ProjectCertificationLiaisonUserFullName;
            string certificationLiaisonEmail = certificationWorkConcept.CertificationLiaisonEmail;
            string certificationLiaisonPhone = certificationWorkConcept.CertificationLiaisonPhone;
            /*
            string certificationLiaison = project.CertificationLiaisonUserFullName;
            string certificationLiaisonEmail = project.CertificationLiaisonEmail;
            string certificationLiaisonPhone = project.CertificationLiaisonPhone;
            */
            if (project.CertifyDate.Year != 1)
            {
                certificationDate = project.CertifyDate.ToString("MM/dd/yyyy");
            }
            else
            {
                certificationDate = certificationWorkConcept.CertificationDateTime.ToString("MM/dd/yyyy");
            }

            string fiscalYear = project.FiscalYear.ToString();
            string advFiscalYear = project.AdvanceableFiscalYear != 0 ? project.AdvanceableFiscalYear.ToString() : "";
            string structureId = Utility.FormatStructureId(structure.StructureId);
            string region = structure.Region;
            string county = structure.County;
            string municipality = structure.Municipality;
            string featureOn = structure.FeatureOn;
            string featureUnder = structure.FeatureUnder;
            string designResourcing = "";
            string estimatedConstructionCost = "";
            string estimatedDesignLevelOfEffort = "";
            string secondaryWorkConcept1 = "";
            string secondaryWorkConcept2 = "";
            string secondaryWorkConcept3 = "";
            string secondaryWorkConcept4 = "";
            string secondaryWorkConcept5 = "";
            string secondaryWorkConcept6 = "";
            string secondaryWorkConcept7 = "";
            string secondaryWorkConcept8 = "";
            string secondaryWorkConcept9 = "";
            string bosCdComments = "";

            if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
            {
                //string certifier = project.CertificationLiaisonUserFullName;

                /*
                if (!String.IsNullOrEmpty(project.PrecertificationLiaisonUserFullName))
                {
                    certifier = project.PrecertificationLiaisonUserFullName;
                }
                else if (!String.IsNullOrEmpty(project.CertificationLiaisonUserFullName))
                {
                    certifier = project.CertificationLiaisonUserFullName;
                }*/

                bosCdComments = String.Format("Transitionally Certified by {0}", certificationLiaison);
                designResourcing = "Contact BOS Design";
                estimatedConstructionCost = "Region to determine";
                estimatedDesignLevelOfEffort = "Region to determine; Contact BOS Design for questions";
                secondaryWorkConcept1 = "Secondary work concepts to be determined by regional bridge maintenance staff";
            }
            else if (project.Status == StructuresProgramType.ProjectStatus.Certified)
            {
                bosCdComments = workConcept.CertificationAdditionalComments;
                designResourcing = workConcept.DesignResourcing;
                estimatedConstructionCost = workConcept.EstimatedConstructionCost > 0 ? workConcept.EstimatedConstructionCost.ToString() : "";
                estimatedDesignLevelOfEffort = workConcept.EstimatedDesignLevelOfEffort > 0 ? workConcept.EstimatedDesignLevelOfEffort.ToString() : "";
                var ewcs = project.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == workConcept.ProjectWorkConceptHistoryDbId
                                                                                        && el.StructureId.Equals(workConcept.StructureId)
                                                                                        && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                        && el.CertificationDateTime == workConcept.CertificationDateTime)
                                                                                        .GroupBy(el => el.WorkConceptCode)
                                                                                        .Select(g => g.First())
                                                                                        .ToList();
                int counter = 0;
                foreach (var ewc in ewcs)
                {
                    counter++;
                    string work = String.Format("({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                    switch (counter)
                    {
                        case 1:
                            secondaryWorkConcept1 = work;
                            break;
                        case 2:
                            secondaryWorkConcept2 = work;
                            break;
                        case 3:
                            secondaryWorkConcept3 = work;
                            break;
                        case 4:
                            secondaryWorkConcept4 = work;
                            break;
                        case 5:
                            secondaryWorkConcept5 = work;
                            break;
                        case 6:
                            secondaryWorkConcept6 = work;
                            break;
                        case 7:
                            secondaryWorkConcept7 = work;
                            break;
                        case 8:
                            secondaryWorkConcept8 = work;
                            break;
                        case 9:
                            secondaryWorkConcept9 = work;
                            break;
                    }
                }
            }


            /*
            string otherInformation1 = "";
            string otherInformation2 = "";
            string otherInformation3 = "";
            string secondaryScope = "";
            string structureDesignResourcing = "";
            string estimatedConstructionCost = "";
            string estimatedDesignLevelOfEffort = "";
            string checkPrimaryScope = "";
            string checkSecondaryScope = "";
            string checkStructureDesignResourcing = "";
            string checkEstimatedConstructionCost = "";
            string checkEstimatedDesignLevelOfEffort = "";
            string pseFromDate = "";
            string pseToDate = "";
            string letFromDate = "";
            string letToDate = "";
            */

            using (var sw = new StreamWriter(xfdfFilePath, false))
            {
                sw.WriteLine($@"<?xml version='1.0' encoding='UTF-8'?>
                                     <xfdf xmlns = 'http://ns.adobe.com/xfdf/' xml:space = 'preserve'>
                                        <f href = '\\mad00fph\n4public\bos\wispt\boscd.pdf'/>
                                        <fields>
                                             <field name = 'DesignId'><value>{designId}</value></field>
                                             <field name = 'ConstructionId'><value>{constructionId}</value></field>
                                             <field name = 'EpseDate'><value>{epseDate}</value></field>
                                             <field name = 'PseDate'><value>{pseDate}</value></field>
                                             <field name = 'DateLet'><value>{letDate}</value></field>
                                             <field name = 'CertificationDate'><value>{certificationDate}</value></field>
                                             <field name = 'StructuresProjectId'><value>{project.ProjectDbId}</value></field>
                                             <field name = 'Fy'><value>{fiscalYear}</value></field>
                                             <field name = 'Afy'><value>{advFiscalYear}</value></field>

                                             <field name = 'StructureId'><value>{structureId}</value></field>
                                             <field name = 'Region'><value>{region}</value></field>
                                             <field name = 'County'><value>{county}</value></field>
                                             <field name = 'Municipality'><value>{municipality}</value></field>
                                             <field name = 'FeatureOn'><value>{featureOn}</value></field>
                                             <field name = 'FeatureUnder'><value>{featureUnder}</value></field>

                                             <field name = 'CertificationLiaison'><value>{certificationLiaison}</value></field>
                                             <field name = 'CertificationLiaisonEmail'><value>{certificationLiaisonEmail}</value></field>
                                             <field name = 'PhoneNumber'><value>{certificationLiaisonPhone}</value></field>

                                             <field name = 'PrimaryWorkConcept'><value>({workConcept.CertifiedWorkConceptCode}) {workConcept.CertifiedWorkConceptDescription}</value></field>  

                                              <field name = 'SecondaryWorkConcept1'><value>{secondaryWorkConcept1}</value></field>                      
                                                 <field name = 'SecondaryWorkConcept2'><value>{secondaryWorkConcept2}</value></field>  
                                                 <field name = 'SecondaryWorkConcept3'><value>{secondaryWorkConcept3}</value></field>  
                                                 <field name = 'SecondaryWorkConcept4'><value>{secondaryWorkConcept4}</value></field>  
                                                 <field name = 'SecondaryWorkConcept5'><value>{secondaryWorkConcept5}</value></field>  
                                                 <field name = 'SecondaryWorkConcept6'><value>{secondaryWorkConcept6}</value></field>  
                                                 <field name = 'SecondaryWorkConcept7'><value>{secondaryWorkConcept7}</value></field>  
                                                 <field name = 'SecondaryWorkConcept8'><value>{secondaryWorkConcept8}</value></field>  
                                                 <field name = 'SecondaryWorkConcept9'><value>{secondaryWorkConcept9}</value></field>  

                                             <field name = 'StructureDesignResourcing'><value>{designResourcing}</value></field>
                                             <field name = 'EstimatedConstructionCost'><value>{estimatedConstructionCost}</value></field>
                                             <field name = 'Loe'><value>{estimatedDesignLevelOfEffort}</value></field>
                                             <field name = 'EpseEarliest'><value>{epseEarliest}</value></field>
                                             <field name = 'EpseLatest'><value>{epseLatest}</value></field>
                                             
                                             
                                             <field name = 'BoscdComments'><value>{bosCdComments}</value></field>
                                             <field name = 'CreatedDate'><value>{dateTime.ToString("yyyy-MM-dd h:mm tt")}</value></field>
                                        </fields>
                                     </xfdf>");
            }
        }

        public void CreatePdfFile(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath)
        {
            FileStream fs = new FileStream(xfdfFilePath, FileMode.Open);
            //XfdfReader xfdfReader = new XfdfReader(new FileStream(xfdfFilePath, FileMode.Open));
            XfdfReader xfdfReader = new XfdfReader(fs);
            PdfReader pdfReader = new PdfReader(pdfFilePath);
            FileStream fileStream = new FileStream(newPdfFilePath, FileMode.Create);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, fileStream);
            pdfStamper.AcroFields.SetFields(xfdfReader);

            if (Directory.Exists(signatureFilePath))
            {
                PushbuttonField signature = pdfStamper.AcroFields.GetNewPushbuttonFromField("certificationSignature");
                signature.Layout = PushbuttonField.LAYOUT_ICON_ONLY;
                signature.ProportionalIcon = true;
                signature.Image = iTextSharp.text.Image.GetInstance(signatureFilePath);
                pdfStamper.AcroFields.ReplacePushbuttonField("certificationSignature", signature.Field);
            }

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader.Close();
            fs.Close();

            try
            {
                File.Delete(xfdfFilePath);
            }
            catch { }
        }

        public string CreateProjectCd(DatabaseService database, Project project, List<Structure> structures, DateTime dateTime, Project fiipsProject)
        {
            List<string> bosCdPdfs = new List<string>();
            tempDir = database.GetTempDirectory();

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            foreach (var workConcept in project.WorkConcepts.OrderBy(wc => wc.StructureId))
            {
                WorkConcept certificationWorkConcept = database.GetStructureCertification(workConcept);
                string xfdfFilePath = Path.Combine(tempDir, String.Format("{0}-{1}.xfdf", workConcept.StructureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
                string newPdfFilePath = Path.Combine(tempDir, String.Format("{0}-{1}.pdf", workConcept.StructureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
                CreateStructureCd(xfdfFilePath, database.GetBosCdTemplate(), newPdfFilePath, database.GetBosCdSignature(), project, structures.Where(st => st.StructureId.Equals(workConcept.StructureId)).First(),
                                        workConcept, dateTime, fiipsProject, false, certificationWorkConcept);
                bosCdPdfs.Add(newPdfFilePath);
            }

            string projectCdPdfFilePath = Path.Combine(tempDir, String.Format("StructuresProject-{0}-{1}.pdf", project.ProjectDbId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
            MergePdfs(bosCdPdfs, projectCdPdfFilePath);

            try
            {
                System.Diagnostics.Process.Start(projectCdPdfFilePath);
            }
            catch (Exception e)
            { }

            return projectCdPdfFilePath;
        }

        public void CreateStructureCd(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath, Project project, 
            Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject, bool openFile = false, WorkConcept certificationWorkConcept = null)
        {
            //CreateXpdfFile(xfdfFilePath, newPdfFilePath, project, structure, workConcept, dateTime, fiipsProject);
            //CreatePdfFile(xfdfFilePath, pdfFilePath, newPdfFilePath, signatureFilePath);

            CreateCdXpdfFile(xfdfFilePath, newPdfFilePath, project, structure, workConcept, dateTime, fiipsProject, certificationWorkConcept);
            CreateCdPdfFile(xfdfFilePath, pdfFilePath, newPdfFilePath, signatureFilePath);

            if (openFile)
            {
                try
                {
                    System.Diagnostics.Process.Start(newPdfFilePath);
                }
                catch (Exception e)
                { }
            }

            //File.Delete(xfdfFilePath);
        }

        public void CreateXpdfFile(string xfdfFilePath, string newPdfFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject)
        {
            //string designID = project.DesignId;
            //string constructionID = project.FosProjectId;
            string epseDate = "";
            string pseDate = "";
            string letDate = project.FiscalYear.ToString();
            //string structureID = structure.StructureId;
            //string region = structure.Region;
            //string county = structure.County;
            string otherInformation1 = "";
            string otherInformation2 = "";
            string otherInformation3 = "";
            string secondaryScope = "";
            string structureDesignResourcing = "";
            string estimatedConstructionCost = "";
            string estimatedDesignLevelOfEffort = "";
            string checkPrimaryScope = "";
            string checkSecondaryScope = "";
            string checkStructureDesignResourcing = "";
            string checkEstimatedConstructionCost = "";
            string checkEstimatedDesignLevelOfEffort = "";
            string pseFromDate = "";
            string pseToDate = "";
            string letFromDate = "";
            string letToDate = "";
            string certificationDate = project.CertifyDate.Year != 1 ? project.CertifyDate.ToString("yyyy-MM-dd") : "";
            string designId = "";
            string certificationAdditionalComments = "";

            if (workConcept.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
            {
                certificationAdditionalComments = String.Format("Transitionally Certified by {0}{1}", project.PrecertificationLiaisonUserFullName, Environment.NewLine);
            }

            certificationAdditionalComments += workConcept.CertificationAdditionalComments;

            if (!String.IsNullOrEmpty(workConcept.CertifiedWorkConceptCode))
            {
                checkPrimaryScope = "Yes";
            }

            if (!String.IsNullOrEmpty(workConcept.SecondaryWorkConcepts))
            {
                checkSecondaryScope = "Yes";
                secondaryScope = workConcept.SecondaryWorkConcepts;
            }

            if (fiipsProject != null)
            {
                designId = fiipsProject.DesignId;

                if (!fiipsProject.EpseDate.Date.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                {
                    epseDate = fiipsProject.EpseDate.Date.ToString("yyyy-MM-dd");
                }

                if (!fiipsProject.PseDate.Date.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                {
                    pseDate = fiipsProject.PseDate.Date.ToString("yyyy-MM-dd");
                }

                if (project.FiscalYear != fiipsProject.FiscalYear)
                {
                    letDate = String.Format("{0} (Fiips-{1})", project.FiscalYear, fiipsProject.FiscalYear);
                }
            }

            if (!workConcept.CertificationDateTime.Date.ToString("yyyy-MM-dd").Equals("0001-01-01"))
            {
                certificationDate = workConcept.CertificationDateTime.ToString("yyyy-MM-dd");
            }

            using (var sw = new StreamWriter(xfdfFilePath, false))
            {
                sw.WriteLine($@"<?xml version='1.0' encoding='UTF-8'?>
                                     <xfdf xmlns = 'http://ns.adobe.com/xfdf/' xml:space = 'preserve'>
                                        <f href = '\\mad00fph\n4public\bos\wispt\boscd.pdf'/>
                                        <fields>
                                             <field name = 'designID'><value>{designId}</value></field>
                                             <field name = 'constructionID'><value>{project.FosProjectId}</value></field>
                                             <field name = 'earlyPSEDate'><value>{epseDate}</value></field>
                                             <field name = 'pseDate'><value>{pseDate}</value></field>
                                             <field name = 'letDate'><value>{letDate}</value></field>
                                             <field name = 'structureID'><value>{structure.StructureId}</value></field>
                                             <field name = 'region'><value>{project.Region}</value></field>
                                             <field name = 'county'><value>{structure.County}</value></field>
                                             <field name = 'municipality'><value>{structure.Municipality}</value></field>
                                             <field name = 'otherInformation1'><value>{otherInformation1}</value></field>
                                             <field name = 'otherInformation2'><value>{otherInformation2}</value></field>
                                             <field name = 'otherInformation3'><value>{otherInformation3}</value></field>

                                             <field name = 'featureOn'><value>{structure.FeatureOn}</value></field>
                                             <field name = 'featureUnder'><value>{structure.FeatureUnder}</value></field>
                                             <field name = 'certificationLiaisonFullName'><value>{workConcept.CertificationLiaisonFullName}</value></field>
                                             <field name = 'certificationLiaisonEmail'><value>{workConcept.CertificationLiaisonEmail}</value></field>
                                             <field name = 'certificationLiaisonPhone'><value>{workConcept.CertificationLiaisonPhone}</value></field>
                                             <field name = 'primaryScope'><value>{workConcept.CertifiedWorkConceptDescription}</value></field>
                                             <field name = 'secondaryScope'><value>{secondaryScope}</value></field>                                        
                                             <field name = 'structureDesignResourcing'><value>{structureDesignResourcing}</value></field>
                                             <field name = 'estimatedConstructionCost'><value>{estimatedConstructionCost}</value></field>
                                             <field name = 'estimatedDesignLevelOfEffort'><value>{estimatedDesignLevelOfEffort}</value></field>
                                         
                                             <field name = 'cbxPrimaryScope'><value>{checkPrimaryScope}</value></field>
                                             <field name = 'cbxSecondaryScope'><value>{checkSecondaryScope}</value></field>
                                             <field name = 'cbxStructureDesignResourcing'><value>{checkStructureDesignResourcing}</value></field>
                                             <field name = 'cbxEstimatedConstructionCost'><value>{checkEstimatedConstructionCost}</value></field>
                                             <field name = 'cbxEstimatedDesignLevelOfEffort'><value>{checkEstimatedDesignLevelOfEffort}</value></field>

                                             <field name = 'pseToDate'><value>{pseToDate}</value></field>
                                             <field name = 'pseFromDate'><value>{pseFromDate}</value></field>
                                             <field name = 'letToDate'><value>{letToDate}</value></field>
                                             <field name = 'letFromDate'><value>{letFromDate}</value></field>

                                             <field name = 'certificationDate'><value>{certificationDate}</value></field>
                                             <field name = 'certificationAdditionalComments'><value>{certificationAdditionalComments}</value></field>                                      
                                             <field name = 'timeStamp'><value>{dateTime}</value></field>
                                        </fields>
                                     </xfdf>");
            }
        }

        public bool MergePdfs(List<string> fileNames, string targetPdf)
        {
            bool merged = true;

            using (FileStream stream = new FileStream(targetPdf, FileMode.Create))
            {
                Document document = new Document();
                PdfCopy pdf = new PdfCopy(document, stream);
                PdfReader reader = null;

                try
                {
                    document.Open();

                    foreach (string file in fileNames)
                    {
                        reader = new PdfReader(file);
                        pdf.AddDocument(reader);
                        reader.Close();
                    }
                }
                catch (Exception)
                {
                    merged = false;

                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                }
            }

            return merged;
        }
    }
}
