//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="FileSystemBasedFeatureParser.cs" company="PicklesDoc">
//  Copyright 2011 Jeffrey Cameron
//  Copyright 2012-present PicklesDoc team and community contributors
//
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using PicklesDoc.Pickles.ObjectModel;

namespace PicklesDoc.Pickles
{
    public class FileSystemBasedFeatureParser
    {
        private readonly IFileSystem fileSystem;

        private readonly FeatureParser parser;

        public FileSystemBasedFeatureParser(FeatureParser parser, IFileSystem fileSystem)
        {
            this.parser = parser;
            this.fileSystem = fileSystem;
        }

        public Feature Parse(string filename)
        {
            Feature feature = null;
            var encoding = this.GetEncoding(filename);
            using (var fileStream = this.fileSystem.FileInfo.FromFileName(filename).OpenRead())
            {
                using (var specificEncoderReader = new StreamReader(fileStream, encoding))
                {
                    try
                    {
                        feature = this.parser.Parse(specificEncoderReader);
                    }
                    catch (FeatureParseException e)
                    {
                        string message =
                            $"There was an error parsing the feature file here: {this.fileSystem.Path.GetFullPath(filename)}" +
                            Environment.NewLine +
                            $"Errormessage was: '{e.Message}'";
                        throw new FeatureParseException(message, e);
                    }
                    specificEncoderReader.Close();

                }
                fileStream.Close();
            }

            return feature;
        }

        private Encoding GetEncoding(string filename)
        {
            var bom = new byte[4];
            using (var file = this.fileSystem.FileInfo.FromFileName(filename).OpenRead())
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default;
        }
    }
}