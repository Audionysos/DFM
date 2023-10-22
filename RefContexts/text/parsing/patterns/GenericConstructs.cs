using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using C = com.audionysos.text.parsing.LanguageConstruct;
using static com.audionysos.text.parsing.BasicMatchers;
using static com.audionysos.text.parsing.patterns.GenericConstructs;
//using com.audionysos.text.parsing.expression;
using Range = com.audionysos.text.parsing.BasicMatchers.Range;

namespace com.audionysos.text.parsing.patterns; 
public class GenericConstructs {
	public class CodeBlock : Range {
		public CodeBlock() : base("{", "}") {}
	}

}

public class CSharpConstucts {

	public class CSharpSource : C {
		public CSharpSource() {
			OPT<UsingStatement>().takeAll();
			OPT(c => {
				c.REQ<NamespaceDefinition>()
				.or<TypeDefiniton>();
			}).takeAll();
		}
	}

	public class UsingStatement : C {
		public UsingStatement() {
			REQ<NamespacUsing>()
				.or<StaticTypeUsing>()
				.or<TypeUsing>();
		}

		public class NamespacUsing : C {
			public NamespacUsing() {
				REQ(Keyword._using).REQ<NamespaceName>();
			}
		}

		public class TypeUsing : C {
			public TypeUsing() {
				REQ(Keyword._using).REQ<UnusedTypeName>()
					.REQ(Operator.assign)
				.REQ<KnownTypeName>();
			}
		}

		public class StaticTypeUsing : C {
			public StaticTypeUsing() {
				REQ(Keyword._using).REQ(Keyword._static).REQ<KnownTypeName>();
			}
		}
	}

	public class NamespaceDefinition : C {
		public NamespaceDefinition() {
			REQ(Keyword._namespace).REQ<NamespaceName>()
			.REQ("{")
				.OPT(c => {
					c.OPT<UsingStatement>().takeAll();
					c.OPT<TypeDefiniton>().takeAll();
				})
			.REQ("}");
		}
	}

	public class TypeDefiniton : C {
		public TypeDefiniton() {
			REQ<ClassDefiniton>()
				.or<StructDfinition>();
		}
	}

	#region Preview classes
	public class B { }
	public class D { }
	public interface I { }

	public class A<T, T2> : D where T : B, I where T2 : I {

	}
	#endregion

	public class ClassDefiniton : C {
		public ClassDefiniton() {
			OPT<AccessModifer>().REQ(Keyword._class).REQ<UnusedTypeName>()
				.OPT<GenericTypesArguments>()
				.OPT(c => c.REQ(":").REQ<InheritionList>())
				.OPT<GenericTypesConstrains>();
			REQ("{");
				OPT<UsingStatement>().takeAll();
				OPT(c => {
					c.REQ<TypeDefiniton>()
					.or<Field>()
					.or<Property>()
					.or<Method>();
				}).takeAll();
			REQ("}");
		}
	}

	public class StructDfinition : C {
		public StructDfinition() {

		}
	}


	public class Operator {
		public static Sequence assign { get; set; } = new Sequence("=");
	}

	public class Keyword {
		public static string _using = "using";
		public static string _static = "static";
		public static string _namespace = "namespace";
		public static string _class = "class";
		public static string _struct = "struct";
	}

	#region Undefined
	public class UnusedTypeName : C {
		public UnusedTypeName() { }
	}

	public class KnownTypeName : C {
		public KnownTypeName() { }
	}

	public class NamespaceName : C {

	}

	public class Field : C { }
	public class Property : C { }
	public class Method : C { }

	public class AccessModifer : C {

	}

	public class GenericTypesArguments : C {

	}

	public class GenericTypesConstrains : C {

	}

	public class InheritionList : C {

	}
	#endregion

}

public class TestClass {

}

//using com.audionysos.text.parsing.expression;


//namespace xxx {
//	public class XYZ {

//	}
//}

//return 10;