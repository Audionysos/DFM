using System;
using static audionysos.gui.Specifier;

namespace audionysos.gui;

public class AccountSpecs {
	private Account s; // subject under test

	public AccountSpecs() {
		givenSubject(s = new Account())
			.method(s.ChangeOwner)
				.returns("old owner").whenGiven("new owner");
	}
}

public static class Specifier {

	internal static SubjectContext current;
	public static SubjectContext<T> givenSubject<T>(T obj) {
		var x = new SubjectContext<T>(obj);
		current = x;
		return x;
	}

	/// <summary>Returns given method as a delegate.</summary>
	public static M method<M>(this object o, M m) where M : Delegate {
		return m;
	}

	public static MethodTestContext<T, R> returns<T, R>(this Func<T, R> t, R r) {
		return new MethodTestContext<T, R>(t, r);
	}

}

public class SubjectContext {
	internal void register<T, R>(MethodTestContext<T, R> methodTestContext) {
		throw new NotImplementedException();
	}
}
public class SubjectContext<T> : SubjectContext {
	public T its { get; }
	public SubjectContext(T t) {
		this.its = t;
	}
}

public abstract class MethodTestContext {
	protected abstract bool test();
}
public class MethodTestContext<T, R> : MethodTestContext {
	private T arg;
	private readonly Func<T, R> m;

	public MethodTestContext<T, R> whenGiven(T arg) {
		this.arg = arg;
		return this;
	}

	protected override bool test() {
		var r = m(arg);
		if (r == null && expectedReturn == null) return true;
		if (r == null || expectedReturn == null) return false;
		return r.Equals(expectedReturn);
	}

	internal R expectedReturn { get; }

	public MethodTestContext(Func<T, R> m, R r) {
		this.m = m;
		this.expectedReturn = r;
		Specifier.current.register(this);
	}
}

//public class Given {
//	public readonly object o;
//	public Given(object o) => this.o = o;
//}
//public class Given<T> : Given {
//	new public T o => (T)base.o;
//	public Given(object o) : base(o) {}
//	public static implicit operator Given<T>(T t)
//		=> new Given<T>(t);

//}


//public delegate void When();
////public delegate void Given();

public class Account {
	public string ChangeOwner(string newOwner) {
		return null;
	}
}

//public class AccountSpecs {
//	private Account sut; // subject under test

//	Given<Account> an_account() => sut = new Account();
//	Given an_account2() => (Given<Account>)(sut = new Account());

//	When changing_owner => () => {
//		var result = sut.ChangeOwner("new owner");
//		It["returns_previous_owner"] = () => result.ShouldBe("old owner");
//	};
//}

//public class AccountSpecs2 {
//	private Given<Account> subject {
//		get => (Given<Account>) sut;
//		set => sut = value.o;
//	}
//	private Account sut; // subject under test

//	Given an_account() => subject = new Account();

//	When changing_owner => () => {
//		var result = sut.ChangeOwner("new owner");
//		It["returns_previous_owner"] = () => result.ShouldBe("old owner");
//	};
//}

//public class AccountSpecs {
//	private Account sut; // subject under test

//	public AccountSpecs() {

//	}

//	Given an_account => () => sut = new Account();

//	When changing_owner => () => {
//		var result = sut.ChangeOwner("new owner");
//		It["returns_previous_owner"] = () => result.ShouldBe("old owner");
//	};
//}

//public static class Specifier {

//	internal static SubjectContext current;
//	public static SubjectContext<T> givenSubject<T>(T obj) {
//		var x = new SubjectContext<T>(obj);
//		current = x;
//		return x;
//	}

//	public static M method<M>(this object o, M m) where M : Delegate {
//		return m;
//	}

//	public static MethodTestContext<T, R> returns<T, R>(this Func<T, R> t, R r) {
//		return new MethodTestContext<T, R>(t, r);
//	}

//	//public static TheIt method { get;  } = new TheIt();
//	public static TheIt It { get; } = new TheIt();
//	public static Action When { get; set; }

//	public static bool ShouldBe(this object t, string x) {
//		return true;
//	}

//}

//public class TheIt {
//	public Action this[string name] {
//		set {

//		}
//	}

//	public Action this[Action m] {
//		get => null;
//		set {

//		}
//	}
//}

