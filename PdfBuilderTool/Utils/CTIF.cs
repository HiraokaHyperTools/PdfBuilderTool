using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Utils
{
    public class CTIF
    {
        public CTIF()
        {

        }

        Stream si;

        LER r;

        public void Read(Stream si)
        {
            this.si = si;
            BinaryReader br = new BinaryReader(si);
            byte b0 = br.ReadByte();
            byte b1 = br.ReadByte();
            bool le;
            if (b0 == 0x49 && b0 == 0x49)
            {
                le = true;
            }
            else if (b0 == 0x4D && b1 == 0x4D)
            {
                le = false;
            }
            else throw new InvalidDataException("byte order");

            r = le ? new LER { br = br } : new BER { br = br };

            if (r.Word() != 42) throw new InvalidDataException("version");

            uint off = r.DWord();

            while (off != 0)
            {
                si.Position = off;
                int cx = r.Word();

                P1 p1 = new P1() { Parent = this };
                for (int x = 0; x < cx; x++)
                {
                    IFD e1 = new IFD();
                    e1.tag = r.Word();
                    e1.ty = r.Word();
                    e1.cnt = (int)r.DWord();
                    e1.val = r.DWord();
                    p1.Add(e1);
                }

                foreach (IFD e1 in p1)
                {
                    if (e1.tag == (int)TTy.IMAGEWIDTH) p1.Width = (int)e1.IntVal;
                    if (e1.tag == (int)TTy.IMAGELENGTH) p1.Height = (int)e1.IntVal;
                    if (e1.tag == (int)TTy.COMPRESSION) p1.Compression = (int)e1.IntVal;
                    if (e1.tag == (int)TTy.XRESOLUTION) p1.HorizontalResolution = GetFloat(e1);
                    if (e1.tag == (int)TTy.YRESOLUTION) p1.VerticalResolution = GetFloat(e1);
                }


                Pages.Add(p1);

                off = r.DWord();
            }
        }

        public List<P1> Pages = new List<P1>();

        public class P1 : List<IFD>
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int Compression { get; set; }
            public float HorizontalResolution { get; set; }
            public float VerticalResolution { get; set; }

            public CTIF Parent;

            public byte[] Strip()
            {
                var STRIPOFFSETS = this.First(q => q.tag == (Int16)TTy.STRIPOFFSETS);
                if (STRIPOFFSETS.ty == 4 && STRIPOFFSETS.cnt == 1)
                {
                    var STRIPBYTECOUNTS = this.First(q => q.tag == (Int16)TTy.STRIPBYTECOUNTS);
                    if (STRIPBYTECOUNTS.ty == 4 && STRIPBYTECOUNTS.cnt == 1)
                    {
                        Parent.si.Position = STRIPOFFSETS.val;
                        byte[] bin = new byte[STRIPBYTECOUNTS.val];
                        if (Parent.si.Read(bin, 0, bin.Length) != bin.Length) throw new EndOfStreamException();
                        return bin;
                    }
                }
                return null;
            }

        }

        public float GetFloat(IFD e1)
        {
            switch (e1.ty)
            {
                case 1:
                case 3:
                case 4:
                    return (int)e1.val;
                case 5:
                    {
                        Int64 old = si.Position;
                        si.Position = e1.val;
                        float r1 = r.DWord();
                        float r2 = r.DWord();
                        si.Position = old;
                        return (float)r1 / r2;
                    }
            }
            throw new InvalidDataException();
        }

        public class IFD
        {
            public int tag { get; set; }
            public int ty { get; set; }
            public int cnt { get; set; }
            public uint val { get; set; }

            public override string ToString()
            {
                return String.Format("{4}({0}), {1}, {2}, {3}", tag, (ty), cnt, val, (TTy)(tag));
            }

            public int IntVal
            {
                get
                {
                    if (cnt == 1)
                    {
                        switch (ty)
                        {
                            case 1:
                            case 3:
                            case 4:
                                return (int)val;
                        }
                    }
                    throw new InvalidDataException();
                }
            }

        }

        enum TTy
        {
            SUBFILETYPE = 254,
            OSUBFILETYPE = 255,
            IMAGEWIDTH = 256,
            IMAGELENGTH = 257,
            BITSPERSAMPLE = 258,
            COMPRESSION = 259,
            PHOTOMETRIC = 262,
            THRESHHOLDING = 263,
            CELLWIDTH = 264,
            CELLLENGTH = 265,
            FILLORDER = 266,
            DOCUMENTNAME = 269,
            IMAGEDESCRIPTION = 270,
            MAKE = 271,
            MODEL = 272,
            STRIPOFFSETS = 273,
            ORIENTATION = 274,
            SAMPLESPERPIXEL = 277,
            ROWSPERSTRIP = 278,
            STRIPBYTECOUNTS = 279,
            MINSAMPLEVALUE = 280,
            MAXSAMPLEVALUE = 281,
            XRESOLUTION = 282,
            YRESOLUTION = 283,
            PLANARCONFIG = 284,
            PAGENAME = 285,
            XPOSITION = 286,
            YPOSITION = 287,
            FREEOFFSETS = 288,
            FREEBYTECOUNTS = 289,
            GRAYRESPONSEUNIT = 290,
            GRAYRESPONSECURVE = 291,
            GROUP3OPTIONS = 292,
            T4OPTIONS = 292,
            GROUP4OPTIONS = 293,
            T6OPTIONS = 293,
            RESOLUTIONUNIT = 296,
            PAGENUMBER = 297,
            COLORRESPONSEUNIT = 300,
            TRANSFERFUNCTION = 301,
            SOFTWARE = 305,
            DATETIME = 306,
            ARTIST = 315,
            HOSTCOMPUTER = 316,
            PREDICTOR = 317,
            WHITEPOINT = 318,
            PRIMARYCHROMATICITIES = 319,
            COLORMAP = 320,
            HALFTONEHINTS = 321,
            TILEWIDTH = 322,
            TILELENGTH = 323,
            TILEOFFSETS = 324,
            TILEBYTECOUNTS = 325,
            BADFAXLINES = 326,
            CLEANFAXDATA = 327,
            CONSECUTIVEBADFAXLINES = 328,
            SUBIFD = 330,
            INKSET = 332,
            INKNAMES = 333,
            NUMBEROFINKS = 334,
            DOTRANGE = 336,
            TARGETPRINTER = 337,
            EXTRASAMPLES = 338,
            SAMPLEFORMAT = 339,
            SMINSAMPLEVALUE = 340,
            SMAXSAMPLEVALUE = 341,
            CLIPPATH = 343,
            XCLIPPATHUNITS = 344,
            YCLIPPATHUNITS = 345,
            INDEXED = 346,
            JPEGTABLES = 347,
            OPIPROXY = 351,
            JPEGPROC = 512,
            JPEGIFOFFSET = 513,
            JPEGIFBYTECOUNT = 514,
            JPEGRESTARTINTERVAL = 515,
            JPEGLOSSLESSPREDICTORS = 517,
            JPEGPOINTTRANSFORM = 518,
            JPEGQTABLES = 519,
            JPEGDCTABLES = 520,
            JPEGACTABLES = 521,
            YCBCRCOEFFICIENTS = 529,
            YCBCRSUBSAMPLING = 530,
            YCBCRPOSITIONING = 531,
            REFERENCEBLACKWHITE = 532,
            XMLPACKET = 700,
            OPIIMAGEID = 32781,
            REFPTS = 32953,
            REGIONTACKPOINT = 32954,
            REGIONWARPCORNERS = 32955,
            REGIONAFFINE = 32956,
            MATTEING = 32995,
            DATATYPE = 32996,
            IMAGEDEPTH = 32997,
            TILEDEPTH = 32998,
            PIXAR_IMAGEFULLWIDTH = 33300,
            PIXAR_IMAGEFULLLENGTH = 33301,
            PIXAR_TEXTUREFORMAT = 33302,
            PIXAR_WRAPMODES = 33303,
            PIXAR_FOVCOT = 33304,
            PIXAR_MATRIX_WORLDTOSCREEN = 33305,
            PIXAR_MATRIX_WORLDTOCAMERA = 33306,
            WRITERSERIALNUMBER = 33405,
            COPYRIGHT = 33432,
            RICHTIFFIPTC = 33723,
            IT8SITE = 34016,
            IT8COLORSEQUENCE = 34017,
            IT8HEADER = 34018,
            IT8RASTERPADDING = 34019,
            IT8BITSPERRUNLENGTH = 34020,
            IT8BITSPEREXTENDEDRUNLENGTH = 34021,
            IT8COLORTABLE = 34022,
            IT8IMAGECOLORINDICATOR = 34023,
            IT8BKGCOLORINDICATOR = 34024,
            IT8IMAGECOLORVALUE = 34025,
            IT8BKGCOLORVALUE = 34026,
            IT8PIXELINTENSITYRANGE = 34027,
            IT8TRANSPARENCYINDICATOR = 34028,
            IT8COLORCHARACTERIZATION = 34029,
            IT8HCUSAGE = 34030,
            IT8TRAPINDICATOR = 34031,
            IT8CMYKEQUIVALENT = 34032,
            FRAMECOUNT = 34232,
            PHOTOSHOP = 34377,
            EXIFIFD = 34665,
            ICCPROFILE = 34675,
            JBIGOPTIONS = 34750,
            GPSIFD = 34853,
            FAXRECVPARAMS = 34908,
            FAXSUBADDRESS = 34909,
            FAXRECVTIME = 34910,
            FAXDCS = 34911,
            STONITS = 37439,
            FEDEX_EDR = 34929,
            INTEROPERABILITYIFD = 40965,
            DNGVERSION = 50706,
            DNGBACKWARDVERSION = 50707,
            UNIQUECAMERAMODEL = 50708,
            LOCALIZEDCAMERAMODEL = 50709,
            CFAPLANECOLOR = 50710,
            CFALAYOUT = 50711,
            LINEARIZATIONTABLE = 50712,
            BLACKLEVELREPEATDIM = 50713,
            BLACKLEVEL = 50714,
            BLACKLEVELDELTAH = 50715,
            BLACKLEVELDELTAV = 50716,
            WHITELEVEL = 50717,
            DEFAULTSCALE = 50718,
            DEFAULTCROPORIGIN = 50719,
            DEFAULTCROPSIZE = 50720,
            COLORMATRIX1 = 50721,
            COLORMATRIX2 = 50722,
            CAMERACALIBRATION1 = 50723,
            CAMERACALIBRATION2 = 50724,
            REDUCTIONMATRIX1 = 50725,
            REDUCTIONMATRIX2 = 50726,
            ANALOGBALANCE = 50727,
            ASSHOTNEUTRAL = 50728,
            ASSHOTWHITEXY = 50729,
            BASELINEEXPOSURE = 50730,
            BASELINENOISE = 50731,
            BASELINESHARPNESS = 50732,
            BAYERGREENSPLIT = 50733,
            LINEARRESPONSELIMIT = 50734,
            CAMERASERIALNUMBER = 50735,
            LENSINFO = 50736,
            CHROMABLURRADIUS = 50737,
            ANTIALIASSTRENGTH = 50738,
            SHADOWSCALE = 50739,
            DNGPRIVATEDATA = 50740,
            MAKERNOTESAFETY = 50741,
            CALIBRATIONILLUMINANT1 = 50778,
            CALIBRATIONILLUMINANT2 = 50779,
            BESTQUALITYSCALE = 50780,
            RAWDATAUNIQUEID = 50781,
            ORIGINALRAWFILENAME = 50827,
            ORIGINALRAWFILEDATA = 50828,
            ACTIVEAREA = 50829,
            MASKEDAREAS = 50830,
            ASSHOTICCPROFILE = 50831,
            ASSHOTPREPROFILEMATRIX = 50832,
            CURRENTICCPROFILE = 50833,
            CURRENTPREPROFILEMATRIX = 50834,
            DCSHUESHIFTVALUES = 65535,
            FAXMODE = 65536,
            JPEGQUALITY = 65537,
            JPEGCOLORMODE = 65538,
            JPEGTABLESMODE = 65539,
            FAXFILLFUNC = 65540,
            PIXARLOGDATAFMT = 65549,
            DCSIMAGERTYPE = 65550,
            DCSINTERPMODE = 65551,
            DCSBALANCEARRAY = 65552,
            DCSCORRECTMATRIX = 65553,
            DCSGAMMA = 65554,
            DCSTOESHOULDERPTS = 65555,
            DCSCALIBRATIONFD = 65556,
            ZIPQUALITY = 65557,
            PIXARLOGQUALITY = 65558,
            DCSCLIPRECTANGLE = 65559,
            SGILOGDATAFMT = 65560,
            SGILOGENCODE = 65561,
        }

        class TTUt
        {
            public static String GetName(int tag)
            {
                switch (tag)
                {
                    case 254: return "SUBFILETYPE";
                    case 255: return "OSUBFILETYPE";
                    case 256: return "IMAGEWIDTH";
                    case 257: return "IMAGELENGTH";
                    case 258: return "BITSPERSAMPLE";
                    case 259: return "COMPRESSION";
                    case 262: return "PHOTOMETRIC";
                    case 263: return "THRESHHOLDING";
                    case 264: return "CELLWIDTH";
                    case 265: return "CELLLENGTH";
                    case 266: return "FILLORDER";
                    case 269: return "DOCUMENTNAME";
                    case 270: return "IMAGEDESCRIPTION";
                    case 271: return "MAKE";
                    case 272: return "MODEL";
                    case 273: return "STRIPOFFSETS";
                    case 274: return "ORIENTATION";
                    case 277: return "SAMPLESPERPIXEL";
                    case 278: return "ROWSPERSTRIP";
                    case 279: return "STRIPBYTECOUNTS";
                    case 280: return "MINSAMPLEVALUE";
                    case 281: return "MAXSAMPLEVALUE";
                    case 282: return "XRESOLUTION";
                    case 283: return "YRESOLUTION";
                    case 284: return "PLANARCONFIG";
                    case 285: return "PAGENAME";
                    case 286: return "XPOSITION";
                    case 287: return "YPOSITION";
                    case 288: return "FREEOFFSETS";
                    case 289: return "FREEBYTECOUNTS";
                    case 290: return "GRAYRESPONSEUNIT";
                    case 291: return "GRAYRESPONSECURVE";
                    case 292: return "GROUP3OPTIONS";
                    //case 292: return "T4OPTIONS";
                    case 293: return "GROUP4OPTIONS";
                    //case 293: return "T6OPTIONS";
                    case 296: return "RESOLUTIONUNIT";
                    case 297: return "PAGENUMBER";
                    case 300: return "COLORRESPONSEUNIT";
                    case 301: return "TRANSFERFUNCTION";
                    case 305: return "SOFTWARE";
                    case 306: return "DATETIME";
                    case 315: return "ARTIST";
                    case 316: return "HOSTCOMPUTER";
                    case 317: return "PREDICTOR";
                    case 318: return "WHITEPOINT";
                    case 319: return "PRIMARYCHROMATICITIES";
                    case 320: return "COLORMAP";
                    case 321: return "HALFTONEHINTS";
                    case 322: return "TILEWIDTH";
                    case 323: return "TILELENGTH";
                    case 324: return "TILEOFFSETS";
                    case 325: return "TILEBYTECOUNTS";
                    case 326: return "BADFAXLINES";
                    case 327: return "CLEANFAXDATA";
                    case 328: return "CONSECUTIVEBADFAXLINES";
                    case 330: return "SUBIFD";
                    case 332: return "INKSET";
                    case 333: return "INKNAMES";
                    case 334: return "NUMBEROFINKS";
                    case 336: return "DOTRANGE";
                    case 337: return "TARGETPRINTER";
                    case 338: return "EXTRASAMPLES";
                    case 339: return "SAMPLEFORMAT";
                    case 340: return "SMINSAMPLEVALUE";
                    case 341: return "SMAXSAMPLEVALUE";
                    case 343: return "CLIPPATH";
                    case 344: return "XCLIPPATHUNITS";
                    case 345: return "YCLIPPATHUNITS";
                    case 346: return "INDEXED";
                    case 347: return "JPEGTABLES";
                    case 351: return "OPIPROXY";
                    case 512: return "JPEGPROC";
                    case 513: return "JPEGIFOFFSET";
                    case 514: return "JPEGIFBYTECOUNT";
                    case 515: return "JPEGRESTARTINTERVAL";
                    case 517: return "JPEGLOSSLESSPREDICTORS";
                    case 518: return "JPEGPOINTTRANSFORM";
                    case 519: return "JPEGQTABLES";
                    case 520: return "JPEGDCTABLES";
                    case 521: return "JPEGACTABLES";
                    case 529: return "YCBCRCOEFFICIENTS";
                    case 530: return "YCBCRSUBSAMPLING";
                    case 531: return "YCBCRPOSITIONING";
                    case 532: return "REFERENCEBLACKWHITE";
                    case 700: return "XMLPACKET";
                    case 32781: return "OPIIMAGEID";
                    case 32953: return "REFPTS";
                    case 32954: return "REGIONTACKPOINT";
                    case 32955: return "REGIONWARPCORNERS";
                    case 32956: return "REGIONAFFINE";
                    case 32995: return "MATTEING";
                    case 32996: return "DATATYPE";
                    case 32997: return "IMAGEDEPTH";
                    case 32998: return "TILEDEPTH";
                    case 33300: return "PIXAR_IMAGEFULLWIDTH";
                    case 33301: return "PIXAR_IMAGEFULLLENGTH";
                    case 33302: return "PIXAR_TEXTUREFORMAT";
                    case 33303: return "PIXAR_WRAPMODES";
                    case 33304: return "PIXAR_FOVCOT";
                    case 33305: return "PIXAR_MATRIX_WORLDTOSCREEN";
                    case 33306: return "PIXAR_MATRIX_WORLDTOCAMERA";
                    case 33405: return "WRITERSERIALNUMBER";
                    case 33432: return "COPYRIGHT";
                    case 33723: return "RICHTIFFIPTC";
                    case 34016: return "IT8SITE";
                    case 34017: return "IT8COLORSEQUENCE";
                    case 34018: return "IT8HEADER";
                    case 34019: return "IT8RASTERPADDING";
                    case 34020: return "IT8BITSPERRUNLENGTH";
                    case 34021: return "IT8BITSPEREXTENDEDRUNLENGTH";
                    case 34022: return "IT8COLORTABLE";
                    case 34023: return "IT8IMAGECOLORINDICATOR";
                    case 34024: return "IT8BKGCOLORINDICATOR";
                    case 34025: return "IT8IMAGECOLORVALUE";
                    case 34026: return "IT8BKGCOLORVALUE";
                    case 34027: return "IT8PIXELINTENSITYRANGE";
                    case 34028: return "IT8TRANSPARENCYINDICATOR";
                    case 34029: return "IT8COLORCHARACTERIZATION";
                    case 34030: return "IT8HCUSAGE";
                    case 34031: return "IT8TRAPINDICATOR";
                    case 34032: return "IT8CMYKEQUIVALENT";
                    case 34232: return "FRAMECOUNT";
                    case 34377: return "PHOTOSHOP";
                    case 34665: return "EXIFIFD";
                    case 34675: return "ICCPROFILE";
                    case 34750: return "JBIGOPTIONS";
                    case 34853: return "GPSIFD";
                    case 34908: return "FAXRECVPARAMS";
                    case 34909: return "FAXSUBADDRESS";
                    case 34910: return "FAXRECVTIME";
                    case 34911: return "FAXDCS";
                    case 37439: return "STONITS";
                    case 34929: return "FEDEX_EDR";
                    case 40965: return "INTEROPERABILITYIFD";
                    case 50706: return "DNGVERSION";
                    case 50707: return "DNGBACKWARDVERSION";
                    case 50708: return "UNIQUECAMERAMODEL";
                    case 50709: return "LOCALIZEDCAMERAMODEL";
                    case 50710: return "CFAPLANECOLOR";
                    case 50711: return "CFALAYOUT";
                    case 50712: return "LINEARIZATIONTABLE";
                    case 50713: return "BLACKLEVELREPEATDIM";
                    case 50714: return "BLACKLEVEL";
                    case 50715: return "BLACKLEVELDELTAH";
                    case 50716: return "BLACKLEVELDELTAV";
                    case 50717: return "WHITELEVEL";
                    case 50718: return "DEFAULTSCALE";
                    case 50719: return "DEFAULTCROPORIGIN";
                    case 50720: return "DEFAULTCROPSIZE";
                    case 50721: return "COLORMATRIX1";
                    case 50722: return "COLORMATRIX2";
                    case 50723: return "CAMERACALIBRATION1";
                    case 50724: return "CAMERACALIBRATION2";
                    case 50725: return "REDUCTIONMATRIX1";
                    case 50726: return "REDUCTIONMATRIX2";
                    case 50727: return "ANALOGBALANCE";
                    case 50728: return "ASSHOTNEUTRAL";
                    case 50729: return "ASSHOTWHITEXY";
                    case 50730: return "BASELINEEXPOSURE";
                    case 50731: return "BASELINENOISE";
                    case 50732: return "BASELINESHARPNESS";
                    case 50733: return "BAYERGREENSPLIT";
                    case 50734: return "LINEARRESPONSELIMIT";
                    case 50735: return "CAMERASERIALNUMBER";
                    case 50736: return "LENSINFO";
                    case 50737: return "CHROMABLURRADIUS";
                    case 50738: return "ANTIALIASSTRENGTH";
                    case 50739: return "SHADOWSCALE";
                    case 50740: return "DNGPRIVATEDATA";
                    case 50741: return "MAKERNOTESAFETY";
                    case 50778: return "CALIBRATIONILLUMINANT1";
                    case 50779: return "CALIBRATIONILLUMINANT2";
                    case 50780: return "BESTQUALITYSCALE";
                    case 50781: return "RAWDATAUNIQUEID";
                    case 50827: return "ORIGINALRAWFILENAME";
                    case 50828: return "ORIGINALRAWFILEDATA";
                    case 50829: return "ACTIVEAREA";
                    case 50830: return "MASKEDAREAS";
                    case 50831: return "ASSHOTICCPROFILE";
                    case 50832: return "ASSHOTPREPROFILEMATRIX";
                    case 50833: return "CURRENTICCPROFILE";
                    case 50834: return "CURRENTPREPROFILEMATRIX";
                    case 65535: return "DCSHUESHIFTVALUES";
                    case 65536: return "FAXMODE";
                    case 65537: return "JPEGQUALITY";
                    case 65538: return "JPEGCOLORMODE";
                    case 65539: return "JPEGTABLESMODE";
                    case 65540: return "FAXFILLFUNC";
                    case 65549: return "PIXARLOGDATAFMT";
                    case 65550: return "DCSIMAGERTYPE";
                    case 65551: return "DCSINTERPMODE";
                    case 65552: return "DCSBALANCEARRAY";
                    case 65553: return "DCSCORRECTMATRIX";
                    case 65554: return "DCSGAMMA";
                    case 65555: return "DCSTOESHOULDERPTS";
                    case 65556: return "DCSCALIBRATIONFD";
                    case 65557: return "ZIPQUALITY";
                    case 65558: return "PIXARLOGQUALITY";
                    case 65559: return "DCSCLIPRECTANGLE";
                    case 65560: return "SGILOGDATAFMT";
                    case 65561: return "SGILOGENCODE";
                }
                return "" + tag;
            }
        }


        class BER : LER
        {
            internal override ushort Word()
            {
                byte b0 = br.ReadByte();
                byte b1 = br.ReadByte();
                return (ushort)((b0 << 8) | b1);
            }

            internal override uint DWord()
            {
                byte b0 = br.ReadByte();
                byte b1 = br.ReadByte();
                byte b2 = br.ReadByte();
                byte b3 = br.ReadByte();
                return (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
            }

            internal override float Float()
            {
                byte[] bin = new byte[4];
                bin[3] = br.ReadByte();
                bin[2] = br.ReadByte();
                bin[1] = br.ReadByte();
                bin[0] = br.ReadByte();
                return BitConverter.ToSingle(bin, 0);
            }
        }
        class LER
        {
            public BinaryReader br;

            internal virtual ushort Word()
            {
                return br.ReadUInt16();
            }

            internal virtual uint DWord()
            {
                return br.ReadUInt32();
            }

            internal virtual float Float()
            {
                return br.ReadSingle();
            }
        }
    }
}
