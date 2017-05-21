![Icon](http://i.imgur.com/GhA9WY5.png?1) 
# PullingHook 
[![Build status](https://ci.appveyor.com/api/projects/status/jhwpf0x1f3eo7x3r?svg=true)](https://ci.appveyor.com/project/lvermeulen/pullinghook) [![license](https://img.shields.io/github/license/lvermeulen/pullinghook.svg?maxAge=2592000)](https://github.com/lvermeulen/pullinghook/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/vpre/pullinghook.svg?maxAge=2592000)](https://www.nuget.org/packages/pullinghook/) [![Coverage Status](https://coveralls.io/repos/github/lvermeulen/PullingHook/badge.svg?branch=master)](https://coveralls.io/github/lvermeulen/PullingHook?branch=master) [![codecov](https://codecov.io/gh/lvermeulen/pullinghook/branch/master/graph/badge.svg)](https://codecov.io/gh/lvermeulen/pullinghook)
 [![Join the chat at https://gitter.im/lvermeulen/pullinghook](https://badges.gitter.im/lvermeulen/pullinghook.svg)](https://gitter.im/lvermeulen/pullinghook?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) ![](https://img.shields.io/badge/.net-4.5.2-yellowgreen.svg) ![](https://img.shields.io/badge/netstandard-1.4-yellowgreen.svg)

PullingHook turns any pull into a push, **remembering the previous values pulled and pushing only when something has changed**. It will pull on a **specified interval** and automatically push the changes.

## Features:
* Fluent builder
* Extensible storage
* Extensible hashing
* Extensible scheduling

## Usage:

* Fluent builder:
~~~~
var pullingHook = PullingHook<MyType, Guid>
	.WithKeyProperty(x => x.Id)
	.WithStorage(new MemoryStorage<decimal>(new Sha1Hasher()))
	.WithScheduler(new FluentPullingHookScheduler<MyType, Guid>())
	.When(TimeSpan.FromSeconds(5), () => new[] { myType1, myType2, myType3 })
	.Then((name, description, changes) => 
	{ 
		Console.WriteLine($"Source name: {name}");
		Console.WriteLine($"Source description: {description}");
		Console.WriteLine($"Added: {changes.Inserts.Count()}");
		Console.WriteLine($"Changed: {changes.Updates.Count()}");
		Console.WriteLine($"Removed: {changes.Deletes.Count()}");
	});

var stoppablePullingHook = pullingHook.Start();

stoppablePullingHook.Stop();
~~~~

* Extensible storage:

Memory-based storage is provided in **PullingHook.Storage.Memory**. To implement your own storage:
~~~~
public interface IPullingSourceStorage<T>
{
	IEnumerable<HashedPair<T>> Retrieve(string key);
	IEnumerable<HashedPair<T>> Store(string key, IEnumerable<T> values);
}
~~~~

A HashedPair<T> is a pair of T with its string hash value.

* Extensible hashing:

SHA256 and SHA1 hashing are provided in **PullingHook.Hasher.Sha256** and **PullingHook.Hasher.Sha1**. To implement your own hashing:
~~~~
public interface IHasher
{
	string Hash(object obj);
}
~~~~

* Extensible scheduling:

Scheduling is provided in **PullingHook.Scheduler.Fluent**. To implement your own scheduling:
~~~~
public interface IPullingScheduler<T, TKeyProperty>
{
	void Start(IPullingHookManager<T, TKeyProperty> pullingHookManager);
	void Stop();
}
~~~~

## Thanks
* [Fishing Hook](https://thenounproject.com/term/fishing-hook/942366) icon by [Rohith M S](https://thenounproject.com/rohithdezinr/) from [The Noun Project](https://thenounproject.com)
 