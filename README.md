# uneaty.core
uNEATy is a fork of the the [UnityNEAT](https://github.com/lordjesus/UnityNEAT) project - which is itself a port of [SharpNEAT](http://sharpneat.sourceforge.net/) from pure C# 4.0 to Unity 4.x and 5 (using Mono 2.6). 
UnityNEAT was created by Daniel Jallov as part of his [master's thesis](http://jallov.com/thesis/) at the [Center for Computer Games Research](http://game.itu.dk/index.php/About) at the IT University in Copenhagen.  Please see the UnityNEAT repo for more information.

== uNEATy ==

uNEATy is a collection of Unity packages to make it easier to implement NEAT [Neuroevolution of Augmenting Topologies](https://en.wikipedia.org/wiki/Neuroevolution_of_augmenting_topologies) neural models within Unity3d.


Here are the current packages available:

1. [uneaty.core](https://github.com/kennelbound/uneaty.core) - Contains the core fork of UnityNEAT and all the low-level NEAT objects.
2. [uneaty.ai](https://github.com/kennelbound/uneaty.ai) - Simple wrapper for defining starting NeuralNetworks more easily, also can be used to "run" a saved NEAT model.
3. [uneaty.visualization](https://github.com/kennelbound/uneaty.visualization) - Unity3d package with components to help render images of the Neural Network.
4. [uneaty.socket.io](https://github.com/kennelbound/uneaty.socket.io) - SocketIO wrapper for serializing and submitting uNEATy objects.


Some example implementation and projects are available as well:

1. [uneaty.runtime](https://github.com/kennelbound/uneaty.runtime) - Example runtime that builds an AI model that can walk using Unity3d's physics engine.
2. [uneaty.viewer](https://github.com/kennelbound/uneaty.viewer) - Example runtime that renders the mental models provided by the uneaty.runtime.


All of the projects are meant to be used with [Projeny](https://github.com/modesttree/Projeny) - a package manager for Unity3d.  You can find an example [$USER\Projeny.yaml here](https://gist.github.com/kennelbound/8277bd266aa97d90e7fb2b42b8d5becc), and a [$PROJECT_ROOT\Projeny.yaml file here](https://gist.github.com/kennelbound/b5e6d825f2d43ec06ceb2dcf8d40d2f7).
