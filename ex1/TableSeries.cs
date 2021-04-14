using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ex1
{
    // class to read csv and store the data.
    //
    // if AvoidIndexErrors = true, then default (0) values
    // will be return where the index/feature doesn't exist
    public class TableSeries
    {
        private float[][] tableValues;
        public readonly string[] featuresStr;
        public readonly List<string> featuresStrAsList;
        private Dictionary<string, int> featuresToInt;
        private string[] timeStepsStr;

        public int RowsLength { get {
                return tableValues.Length;
            } }
        public int ColumnsLength { get {
                return featuresStr.Length; 
            } }
        public int FeaturesLength { get { return ColumnsLength; } }
        public bool AvoidIndexErrors { get; set; }

        // get csvfile to read the data from and defaultFeatures == default title to columns,
        // if the file first line doesnt contain it
        public TableSeries(string csvfile, string defaultFeatures = "", bool avoidIndexErrors = true)
        {
            this.AvoidIndexErrors = avoidIndexErrors;

            // get file lines number
            int valuesLinesNumber = 0;
            using (FileStream fs = File.OpenRead(csvfile))
            {
                var sr = new StreamReader(fs);
                while (sr.ReadLine() != null) valuesLinesNumber++;
            }

            using (FileStream fs = File.OpenRead(csvfile))
            {
                // read first line and decide if to use it as features line or as values line
                var sr = new StreamReader(fs);
                string featuresLine = sr.ReadLine();
                char c = featuresLine[0];
                string values;
                if (Char.IsDigit(c) || c == '.')
                {
                    values = featuresLine;
                    featuresLine = defaultFeatures;
                }
                else
                {
                    values = sr.ReadLine();
                    valuesLinesNumber--;
                }

                // decide features final names
                this.featuresStr = featuresLine.Split(',');
                this.featuresToInt = new Dictionary<string, int>();
                for (int k = 0; k < this.featuresStr.Length; k++)
                {
                    if (!this.featuresToInt.ContainsKey(this.featuresStr[k]))
                        this.featuresToInt.Add(this.featuresStr[k], k);
                    String without0 = featuresStr[k];
                    if (without0.Length > 3 && without0.Substring(without0.Length - 3) == "[0]") {
                        without0 = without0.Substring(0, without0.Length - 3);
                        if (!this.featuresToInt.ContainsKey(without0))
                            this.featuresToInt.Add(without0, k);
                    }
                }

                // read the data to the correct cell
                this.timeStepsStr = new string[valuesLinesNumber];
                this.tableValues = new float[valuesLinesNumber][];
                for (int h = 0; h < this.tableValues.Length; h++)
                    this.tableValues[h] = new float[this.featuresStr.Length];

                int i = 0; String[] valuesBeforeParse;

                this.timeStepsStr[i] = values;
                valuesBeforeParse = values.Split(',');
                for (int j = 0; j < valuesBeforeParse.Length; j++)
                    this.tableValues[i][j] = float.Parse(valuesBeforeParse[j]);

                while (!sr.EndOfStream)
                {
                    i++;
                    values = sr.ReadLine();
                    this.timeStepsStr[i] = values;
                    valuesBeforeParse = values.Split(',');
                    for (int j = 0; j < valuesBeforeParse.Length; j++)
                        this.tableValues[i][j] = float.Parse(valuesBeforeParse[j]);
                }
            }
            featuresStrAsList = featuresStr.ToList();
        }

        public float getCell(int i, int j)
        {
            bool idxErr = i < 0 || j < 0 || i >= tableValues.GetLength(0) || j >= tableValues[i].GetLength(0);
            if (AvoidIndexErrors && idxErr) return 0;
            return this.tableValues[i][j];
        }
        public float getCell(int timeStep, string feature)
        {
            if (AvoidIndexErrors && !featuresToInt.ContainsKey(feature)) return 0;
            return getCell(timeStep, this.featuresToInt[feature]);
        }
        public float[] getColumn(string feature)
        {
            if (AvoidIndexErrors && !featuresToInt.ContainsKey(feature)) return new float[this.tableValues.Length];
            return getColumn(this.featuresToInt[feature]);
        }
        public float[] getColumn(int j)
        {
            try
            {
                float[] temp = new float[this.tableValues.Length];
                for (int i = 0; i < this.tableValues.Length; i++)
                    temp[i] = this.tableValues[i][j];
                return temp;
            }
            catch
            {
                if (AvoidIndexErrors) return new float[this.tableValues.Length];
                throw;
            }
        }
        public float[] getRowArray(int timeStep)
        {
            if (AvoidIndexErrors && (timeStep < 0 || timeStep >= tableValues.GetLength(0))) return new float[this.ColumnsLength];
            return this.tableValues[timeStep];
        }
        public string getRowAsString(int timeStep)
        {
            if (AvoidIndexErrors && (timeStep < 0 || timeStep >= tableValues.GetLength(0)))
            {
                var temp = new String[this.ColumnsLength];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = "0";
                return String.Join(",", temp);
            }
            return this.timeStepsStr[timeStep];
        }
    }
}
