/**
 * Author: Jeff Leupold
 * Homework 6
 * Due Date: 2021-05-27
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Homework6 {
    class Program {

        public struct mProp {
            public bool isSymmetric;
            public bool isWeighted;
            public int rowCount;
        }
        /**
         * Test Cases:
         * adj4_weights: testing to handle values other 0/1. Need my data matrix to be double instead of int
         * adj_tallRect: matrix needs to be square so testin this error
         * adj_wideRect: testing a different error
         * adj_textValues: test to make sure the program handles the exception for string values instead of numeric
         * adj_allZeros: testing the degenerate case
         * adj_blank: testing a blank file to see if the program properly handles the exception
         */
        static string[] fileName = { "adj1.txt", "adj2.txt", "adj3.txt", "adj4.txt", "adj4_weights.txt", 
                                     "adj_tallRect.txt", "adj_wideRect.txt", "adj_textValues.txt", "adj_allZeros.txt", "adj_blank.txt" };
        static void Main(string[] args) {
            string filePath = $"{GetApplicationRoot()}\\{fileName[9]}";
            string outputFile = $"{GetApplicationRoot()}\\output.dot";
            Console.WriteLine(filePath);
            List<List<double>> data = importMatrix(filePath);

            if (data != null && qualityCheck(data)) {
                string response = buildDotFile(data);
                exportFile(outputFile, response);
                Console.WriteLine("++++++++++ EXIT SUCCESS! ++++++++++");
            }
            else
                Console.WriteLine("========== EXIT FAILURE! ==========");
        }

        public static string buildDotFile(List<List<double>> matrix) {
            string connector, graphType;
            mProp mp = matrixProperties(matrix);
            StringBuilder dotString = new();
            string nodeString = getNodeString(matrix);
            
            if (mp.isSymmetric) {
                connector = "--";
                graphType = "graph";
                dotString.Append(graphType);
                dotString.AppendLine(" {");
                dotString.AppendLine(nodeString);
                for (int i = 0; i < mp.rowCount; i++) {
                    for (int j = 0; j <= i; j++) {
                        if (matrix[i][j] != 0) {
                            dotString.Append((char)(i+97));
                            dotString.Append($" {connector} ");
                            dotString.Append((char)(j+97));
                            if (mp.isWeighted)
                                dotString.Append($"[label=\"{matrix[i][j]}\",weight=\"{matrix[i][j]}\"]");
                            dotString.AppendLine(";");
                        }
                    }
                }
                dotString.AppendLine("}");
            }
            else {
                connector = "->";
                graphType = "digraph";
                dotString.Append(graphType);
                dotString.AppendLine(" {");
                dotString.AppendLine(nodeString);
                for (int i = 0; i < mp.rowCount; i++) {
                    for (int j = 0; j < mp.rowCount; j++) {
                        if (matrix[i][j] != 0) {
                            dotString.Append((char)(i+97));
                            dotString.Append($" {connector} ");
                            dotString.Append((char)(j+97));
                            if (mp.isWeighted)
                                dotString.Append($"[label=\"{matrix[i][j]}\",weight=\"{matrix[i][j]}\"]");
                            dotString.AppendLine(";");
                        }
                    }
                }
                dotString.AppendLine("}");
            }

            return dotString.ToString();
        }

        public static string getNodeString(List<List<double>> matrix) {
            string output = "";
            for (int i = 0; i < matrix.Count; i++) {
                output += (char)(i + 97) + ";";
            }
            return output;
        }

        public static string GetApplicationRoot() {
            var exePath =   Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            
            return appRoot;
        }

        public static void exportFile(string filePath, string text) {
            using (StreamWriter sw = new StreamWriter(filePath)) {
                sw.WriteLine(text);
            }
        }

        public static List<List<double>> importMatrix(string file) {
            StreamReader sr = new StreamReader(file);
            List<List<double>> data = new List<List<double>>();

            try {
                string line = sr.ReadLine();
                while (line != null) {
                    Console.WriteLine(line);
                    data.Add(line.Split('\t').Select(double.Parse).ToList<double>());

                    line = sr.ReadLine();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                data = null;
            }
            finally {
                if (sr != null)
                    sr.Close();
            }

            return data;
        }

        public static bool qualityCheck(List<List<double>> matrix) {
            if (matrix.Count == 0) {
                Console.WriteLine("File was empty.");
                return false;
            }

            if (!isSquare(matrix)) {
                Console.WriteLine("The adjacency matrix was not square.");
                return false;
            }

            if (!hasValidValues(matrix)) {
                Console.WriteLine("The adjacency matrix contains values less than 0.");
            }

            return true;
        }
        public static bool isSquare(List<List<double>> matrix) {
            int rowCount = matrix.Count;
            foreach (List<double> col in matrix) {
                if (col.Count != rowCount)
                    return false;
            }
            return true;
        }

        public static bool hasValidValues(List<List<double>> matrix) {
            foreach (List<double> row in matrix) {
                foreach (double cell in row) {
                    if (cell < 0)
                        return false;
                }
            }
            return true;
        }
        /**
         * Determines if graph is directed or not
         */
        public static mProp matrixProperties(List<List<double>> matrix) {
            mProp prop;
            prop.isSymmetric = true;
            prop.isWeighted = false;
            prop.rowCount = matrix.Count;

            //matrix.Count = # of rows
            for (int i = 0; i < prop.rowCount; i++) {
                for (int j = 0; j < i; j++) {
                    if (matrix[i][j] != matrix[j][i])
                        prop.isSymmetric = false;

                    if (matrix[i][j] != 0 && matrix[i][j] != 1)
                        prop.isWeighted = true;
                }
            }

            return prop;
        }
    }
}
