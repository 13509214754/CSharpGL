﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGL {
    /// <summary>
    /// 
    /// </summary>
    public class ObjVNFParser {
        private readonly ObjParserBase[] parserList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quad2triangle"></param>
        public ObjVNFParser(bool quad2triangle) {
            var generalityParser = new GeneralityParser();
            var meshParser = new MeshParser();
            var normalParser = new NormalParser();
            var texCoordParser = new TexCoordParser();
            var tangentParser = new TangentParser();
            var locationParser = new LocationParser();
            if (quad2triangle) {
                this.parserList = new ObjParserBase[] { generalityParser, meshParser, normalParser, texCoordParser, new Quad2TriangleParser(), tangentParser, locationParser, };
            }
            else {
                this.parserList = new ObjParserBase[] { generalityParser, meshParser, normalParser, texCoordParser, tangentParser, locationParser, };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ObjVNFResult Parse(string filename) {
            ObjVNFResult result = new ObjVNFResult();
            var context = new ObjVNFContext(filename);
            try {
                foreach (var item in this.parserList) {
                    item.Parse(context);
                }

                result.Mesh = context.Mesh;
            }
            catch (Exception ex) {
                result.Error = ex;
            }

            return result;
        }
    }
}
