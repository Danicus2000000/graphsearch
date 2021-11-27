﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace graphsearch
{
    public class GetInformation
    {
        public enum sortingAlgorithm//enum for sorting algorithm type
        {
            Null,
            Dijkstra,
            AStar,
            BF
        }
        public enum FileParseMode 
        {
            Null,
            Nodes,
            Edges
        }
        /// <summary>
        /// Gets the information from the console arguments
        /// </summary>
        /// <param name="pArgs">The console arguments</param>
        /// <param name="startNode">The Start Node</param>
        /// <param name="endNode">The End Node</param>
        /// <param name="fileToRead">The File to read from</param>
        /// <param name="fileToWrite">The file to write to</param>
        /// <param name="outputToConsole">Whether the console should be outputted to</param>
        /// <param name="debugInfo">Whether debug infromation should be shown</param>
        /// <param name="chosenAlgorithm">The name of the chosen algorithm</param>
        /// <param name="failedBuild">Whether there has been an error during information collection</param>
        public void GrabData(string[] pArgs, out string startNode, out string endNode, out string fileToRead, out string fileToWrite, out bool outputToConsole, out bool debugInfo, out sortingAlgorithm chosenAlgorithm, out bool failedBuild)
        {
            startNode = null;//intialises variables to default values
            endNode = null;
            fileToRead = null;
            fileToWrite = null;
            outputToConsole = true;
            debugInfo = false;
            chosenAlgorithm = sortingAlgorithm.Null;
            failedBuild = false;
            bool helpAlreadyShown = false; //variable to stop help showing multiple times if -h flag is used more than once in args
            if (pArgs.Length != 0)//if there are any console arguments
            {
                for (int i = 0; i < pArgs.Length; i++)//loop through all console arguments
                {
                    try//catch any errors that may occur
                    {
                        switch (pArgs[i])
                        {
                            case "-s"://if arg is -s set start node
                                startNode = pArgs[i + 1];
                                break;
                            case "-e"://if arg is -e set end node
                                endNode = pArgs[i + 1];
                                break;
                            case "-f"://if arg is -f set file to read
                                fileToRead = pArgs[i + 1];
                                break;
                            case "-o":
                            case "-O":
                                if (pArgs[i] == "-o" || pArgs[i] == "-O")//if -o or -O flag set file output
                                {
                                    fileToWrite = pArgs[i + 1];
                                }
                                if (pArgs[i] == "-o")//if -o flag don't output to console
                                {
                                    outputToConsole = false;
                                }
                                if (pArgs[i] == "-O") //if -O flag output to console
                                {
                                    outputToConsole = true;
                                }
                                break;
                            case "-d":
                                debugInfo = true;//if -d flag enable debug info
                                break;
                            case "-a"://if a flag find sorting algorithm or if inocorrect entered throw an exception
                                if (pArgs[i + 1].ToUpper() == "DIJKSTRA")
                                {
                                    chosenAlgorithm = sortingAlgorithm.Dijkstra;
                                }
                                else if (pArgs[i + 1].ToUpper() == "ASTAR")
                                {
                                    chosenAlgorithm = sortingAlgorithm.AStar;
                                }
                                else if (pArgs[i + 1].ToUpper() == "BF")
                                {
                                    chosenAlgorithm = sortingAlgorithm.BF;
                                }
                                break;
                            case "-h":
                                if (!helpAlreadyShown)//shows help unless it has already been shown in which case it is ignored
                                {
                                    Console.WriteLine("-s <number or string>: Specify starting node, either as an integer or a string.  If a string, it must be in quotes, such as \"A\" or \"Norwich\".");
                                    Console.WriteLine("-e <number or string>: Specify end node either as an integer or a string.  If a string, it must be in quotes, such as \"Z\" or \"Zetland\".");
                                    Console.WriteLine("-a BF|DIJKSTRA|ASTAR: Use the relevant algorithm (brute force, Dijkstra's or A*)");
                                    Console.WriteLine("-f <filename>: read data from <filename> as the input");
                                    Console.WriteLine("-o <filename>: Write the output to file <filename> only");
                                    Console.WriteLine("-O <filename>: Write the output to both the console and file <filename>");
                                    Console.WriteLine("-d: Output debugging information to the console.");
                                    Console.WriteLine("-h: Shows the help dialog");
                                    helpAlreadyShown = true;
                                }
                                break;
                        }
                    }
                    catch (Exception)//if there is an exception fail the build
                    {
                        failedBuild = true;
                    }
                }

            }
        }

        /// <summary>
        /// Checks whether the program is okay to run
        /// </summary>
        /// <param name="failedBuild">If the Build Failed</param>
        /// <param name="startNode">The start node</param>
        /// <param name="endNode">The end node</param>
        /// <param name="fileToRead">The file to read</param>
        /// <param name="chosenSort">The chosen sorting method</param>
        /// <returns>A bool representing whether the program can begin</returns>
        public bool CanRun(bool failedBuild,string startNode,string endNode,string fileToRead, sortingAlgorithm chosenSort) 
        {
            if (!failedBuild && startNode != null && endNode != null && fileToRead != null && chosenSort!=sortingAlgorithm.Null) //if the build succeeds check file location given is valid
            {
                if (File.Exists(fileToRead)) 
                {
                    return true;
                }
                else 
                {
                    Console.WriteLine("The requested file does not exist!");
                    return false;
                }
            }
            else //else warn that the information given is incorrect or formatted incorrectly
            {
                Console.WriteLine("One or more required arguments where missing or invalid!");
                return false;
            }
        }

        /// <summary>
        /// Reads the node file and stores them as required
        /// </summary>
        /// <param name="fileToRead">The file to read</param>
        /// <param name="nodes">The list of nodes recieved from the file</param>
        /// <returns>A bool representing whether the file is formatted correctly</returns>
        public bool ParseFile(string fileToRead, out List<Node> nodes, out int[,] adjacencyMatrix) 
        {
            FileParseMode fileMode = FileParseMode.Null;
            nodes = new List<Node>();//contains all the nodes in the diagram
            adjacencyMatrix = null;
            try
            {
                foreach (string line in File.ReadAllLines(fileToRead))//foreach line of info in the file
                {
                    string[] lineInfo = line.Split(",");//split it by commas as required
                    for(int i=0; i<lineInfo.Length;i++) //removes any whitespace that could be in the file acccidentally
                    {
                        lineInfo[i].Replace(" ","");
                    }
                    if (lineInfo[0].ToLower() == "nodes")//if the word nodes is seen change the next lines to parse into nodes
                    {
                        fileMode = FileParseMode.Nodes;
                    }
                    else if (lineInfo[0].ToLower() == "edges")//if the word edges is seen change the next lines to parse into edges
                    {
                        fileMode = FileParseMode.Edges;
                    }
                    else//if the line is information
                    {
                        if (fileMode == FileParseMode.Nodes) //if we are currently parsing for nodes
                        {
                            nodes.Add(new Node(lineInfo[1].Replace("\"",""),Convert.ToInt32(lineInfo[0]),Convert.ToInt32(line[2]),Convert.ToInt32(lineInfo[3])));//Adds a new node with the details given in the file    
                        }
                        else if (fileMode == FileParseMode.Edges) //if we are currently parsing for edges
                        {
                            bool foundNode1 = false;//stores if each node has been found
                            bool foundNode2 = false;
                            if (adjacencyMatrix == null) 
                            {
                                adjacencyMatrix = new int[nodes.Count, nodes.Count];
                            }
                            foreach (Node node in nodes)//loop through all nodes and add the mentioned edge the first number is the node that you can traverse to the second is the cost to traverse this path
                            {
                                if (node.nodeIndex == Convert.ToInt32(lineInfo[0]))
                                {
                                    node.paths.Add(Convert.ToInt32(lineInfo[1]), Convert.ToInt32(lineInfo[2]));
                                    adjacencyMatrix[Convert.ToInt32(lineInfo[0])-1,Convert.ToInt32(lineInfo[1])-1]=Convert.ToInt32(lineInfo[2]);
                                    foundNode1 = true;
                                }
                                else if (node.nodeIndex == Convert.ToInt32(lineInfo[1]))
                                {
                                    node.paths.Add(Convert.ToInt32(lineInfo[0]), Convert.ToInt32(lineInfo[2]));
                                    adjacencyMatrix[Convert.ToInt32(lineInfo[1])-1, Convert.ToInt32(lineInfo[0])-1] = Convert.ToInt32(lineInfo[2]);
                                    foundNode2 = true;
                                }
                                if (foundNode1 && foundNode2)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex) //tells prorgam the operation failed
            {
                Console.WriteLine("The data in the file given was not formatted correctly!");
                return false;
            }
        }

        /// <summary>
        /// Assigns the start and end node bools in the list
        /// </summary>
        /// <param name="startNode">The start node</param>
        /// <param name="endNode">The end node</param>
        /// <param name="nodes">The list of all nodes</param>
        /// <param name="trueStartNode">A refrence to the start node in the list of nodes</param>
        /// <param name="trueEndNode">A refrence to the end node in the list of nodes</param>
        /// <returns>A bool representing whether the start and end node strings occur in the file</returns>
        public bool SetStartNodeAndEndNode(string startNode,string endNode, List<Node> nodes, out Node trueStartNode, out Node trueEndNode)
        {
            bool foundStart = false;//used to ensure we do not have to loop through all nodes every time to save time
            bool foundEnd = false;
            trueStartNode = null;
            trueEndNode = null;
            foreach(Node node in nodes) //sets start and end node upon finding them
            {
                if (startNode == node.name) 
                {
                    node.isStartNode = true;
                    trueStartNode = node;
                    foundStart = true;
                }
                else if(endNode == node.name) 
                {
                    node.isEndNode = true;
                    trueEndNode = node;
                    foundEnd = true;
                }
                if(foundStart && foundEnd) 
                {
                    break;
                }
            }
            if(!foundStart || !foundEnd) //ensures that the start node and end node given are in the file by returning false if they are not found
            {
                Console.WriteLine("The start node and end nodes given do not match any given in the file!");
                return false;
            }
            else 
            {
                return true;
            }
        }
    }
}
