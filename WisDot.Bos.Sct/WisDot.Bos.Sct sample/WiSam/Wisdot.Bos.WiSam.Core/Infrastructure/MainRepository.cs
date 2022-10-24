using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Infrastructure.Interfaces;

namespace Wisdot.Bos.WiSam.Core.Infrastructure
{
    public class MainRepository : IMainRepository
    {
        public NeedsAnalysisFile CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes needsAnalysisFileType, string baseDirectory, string fileExtension)
        {
            NeedsAnalysisFile needsAnalysisFile = null;
            string filePath = GetRandomFileName(baseDirectory, fileExtension);
            needsAnalysisFile = new NeedsAnalysisFile(String.Format("{0}-{1}", needsAnalysisFileType.ToString(), GetFileName(filePath)), filePath,
                                                        fileExtension, DateTime.Now, needsAnalysisFileType);

            return needsAnalysisFile;
        }

        public string GetRandomFileName(string baseDir, string newFileExt)
        {
            string newPath = Path.GetRandomFileName();
            string fileExt = Path.GetExtension(newPath);
            newPath = newPath.Replace(fileExt, newFileExt);
            newPath = Path.Combine(baseDir, newPath);
            return newPath;
        }

        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public void SetAnalysisFiles(NeedsAnalysisInput currentNeedsAnalysisInput)
        {
            // Analysis files are based on analysis type
            currentNeedsAnalysisInput.AnalysisFiles.Clear();
            switch (currentNeedsAnalysisInput.AnalysisType)
            {
                case WisamType.AnalysisTypes.Optimal:
                //case WisamType.AnalysisTypes.StructureDeckReplacement:
                //case WisamType.AnalysisTypes.FlexibleOptimal:
                case WisamType.AnalysisTypes.LocalProgram:
                case WisamType.AnalysisTypes.MetaManager:
                    if (currentNeedsAnalysisInput.CreateBridgeInventoryFile)
                    {
                        currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.Inventory, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                    }

                    // Unconstrained Needs Analysis
                    if (currentNeedsAnalysisInput.RunUnconstrainedAnalysis)
                    {
                        currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.ProgramUnconstrained, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".xlsx"));

                        // Debug File
                        if (currentNeedsAnalysisInput.CreateDebugFile)
                        {
                            currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.ConditionUnconstrained, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                        }

                        if (currentNeedsAnalysisInput.ShowPriorityScore)
                        {
                            currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.PriorityUnconstrained, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                        }
                    }

                    // Constrained Needs Analysis
                    if (currentNeedsAnalysisInput.ApplyBudget)
                    {
                        currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.ProgramConstrained, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".xlsx"));

                        // Debug File
                        if (currentNeedsAnalysisInput.CreateDebugFile)
                        {
                            currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.ConditionConstrained, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                        }

                        if (currentNeedsAnalysisInput.ShowPriorityScore)
                        {
                            currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.PriorityConstrained, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                        }
                    }

                    // Input File
                    currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.InputFile, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".xml"));

                    // Additional File
                    if (currentNeedsAnalysisInput.AnalysisType.Equals(WisamType.AnalysisTypes.MetaManager))
                    {
                        currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.MetaManager, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                    }
                    else if (currentNeedsAnalysisInput.AnalysisType.Equals(WisamType.AnalysisTypes.LocalProgram))
                    {
                        currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.LocalProgram, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                    }

                    break;

                case WisamType.AnalysisTypes.CoreData:
                    currentNeedsAnalysisInput.AnalysisFiles.Add(CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes.CoreData, currentNeedsAnalysisInput.BaseDirectoryOfAnalysisFiles, ".csv"));
                    break;
            }
        }

        public LinkLabel WriteResult(int fileCounter, NeedsAnalysisFile analysisFile, int pointX, int pointY)
        {
            LinkLabel lnkLbl = new LinkLabel();
            lnkLbl.Width = 200;
            lnkLbl.Text = String.Format("{0}-{1}", fileCounter, analysisFile.NeedsAnalysisFileType.ToString());
            //lnkLbl.Width = lnkLbl.Text.Length + 1;
            lnkLbl.Tag = analysisFile.FilePath;
            lnkLbl.Location = new Point(pointX, pointY);
            return lnkLbl;
        }

        public bool ValidateStartEndYears(string startYearTxt, string endYearTxt)
        {
            int startYear, endYear;
            bool startYearResult = int.TryParse(startYearTxt, out startYear);
            bool endYearResult = int.TryParse(endYearTxt, out endYear);

            if (!startYearResult || !endYearResult)
            {
                MessageBox.Show("Enter valid start and end years.");
                return false;
            }

            if (startYear < DateTime.Now.Year)
            {
                MessageBox.Show(String.Format("Start year must be at least {0}.", DateTime.Now.Year));
                return false;
            }

            if (endYear < startYear)
            {
                MessageBox.Show("End year must be equal or greater than start year.");
                return false;
            }

            return true;
        }

        public bool ValidateStructureId(string structureIdsTxt)
        {
            if (structureIdsTxt.Equals(""))
            {
                MessageBox.Show("Enter structure ID(s).");
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidateStateOrLocal(bool stateOwned, bool localOwned)
        {
            bool valid = true;

            if (!stateOwned && !localOwned)
            {
                MessageBox.Show("Check state and/or local structures.");
                valid = false;
            }

            return valid;
        }

        public List<string> ConvertStringToList(string stringToConvert)
        {
            List<string> newList = stringToConvert.Split(new string[] { ",", " ", ";", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return newList;
        }
    }
}
