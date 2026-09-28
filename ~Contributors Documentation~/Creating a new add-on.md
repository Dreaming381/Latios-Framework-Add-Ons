# Creating a New Add-On

This guide will walk you through the steps to create a new add-on.

## Setting Up the Environment

First, you will need a GitHub account. With it, you should fork the Latios
Framework Add-Ons repository for yourself.

Second, you will need to clone this repository (or add it as a submodule)
directly inside the *Packages* folder of your Unity project.

Third, in the *AddOns* folder of this repository in your project, create a new
folder named after your add-on.

Fourth, decide on a scripting define symbol for your add-on. The convention is
`LATIOS_ADDON_<yourAddOnName>`.

## Creating Assembly Definitions

Create one or more assembly definitions for your add-on. Each asmdef should
specify a root namespace. It is common practice for the name of the assembly to
match this root namespace. The convention is `Latios.<YourAddOnName>`.

Next, populate the *Assembly Definition References* section. Typically, you will
depend on the following:

-   Latios.Core
-   Unity.Burst
-   Unity.Collections
-   Unity.Entities
-   Unity.Entities.Hybrid (authoring)
-   Unity.Mathematics
-   (If you touch transforms)
    -   Latios.Transforms
    -   Unity.Transforms

In the *Define Constraints* section, add the scripting define for your add-on.
Optionally, you can also add constraints for any additional add-ons your add-on
depends on.

## Populating your Add-On

Your add-on folder should exclusively be composed of C\# files, assembly
definitions and references, and anything else that ties into the compilation.
Assets such as prefabs, Scriptable Objects, meshes, materials, or shaders should
not be included. If you need these, it is best to create a sample or an editor
tool to inject them into a user’s project. If you create a sample, please modify
the repository root’s package.json to include it.

Additionally, you should not leave any of your folders empty. (It is fine during
development, but not for end users.) And you should not have an assembly
definition without any associated C\# files.

It is essential that your add-on is fully inert until the user enables it via
scripting define. That’s the reason for these rules.

### Other conventions

It is generally good practice to put any authoring components and other
utilities in an `Authoring` namespace, and to put systems in a `Systems`
namespace. These should be suffixes of your root namespace if you only have a
single assembly definition.

In any authoring component you create, it is a good idea to add an
`[AddComponentMenu]` attribute and specify a path. Additionally, the last name
in the path (what follows after the last forward slash) is the authoring
component’s presentation name. This should include the name of the add-on, as
only the presentation name will be shown in search results. A good way to do
this is to put the add-on name in parenthesis as a suffix like this: `“Latios/My
Cool Add-On/My Authoring (My Cool Add-On)”`.

### Incomplete Add-Ons

Your add-on does not need to be functional when you first attempt to add it. As
long as it is inert, and the README documents its incomplete status, you can
still push it out to the public. You may wish to do this to make it easier for
others to collaborate and contribute and help get the add-on across the finish
line.

## Licensing your Add-On

By default, add-ons are licensed under the Unity Companion License. However, you
may override this by providing a new LICENSE file in your add-on’s directory.
Besides UCL, MIT and BSD licenses are good choices.

Additionally, if your add-on includes code you retrieved from elsewhere, you
must provide a THIRD PARTY NOTICES file in your add-on’s directory. You must do
this even if you elect to default to the UCL license, with an exception if the
retrieved code is also licensed under UCL.

## Adding Documentation

You must provide a README file for your add-on. There are two template files
provided which you can use. The “Instructional” variant provides extra info to
help guide you in italic text. If you directly copy from this one, you should
delete all the original italic text.

You can optionally include separate documentation pages in a \~Documentation\~
folder, or you can link to a separate webpage.

If you would like your add-on to be versioned, you can add a CHANGELOG file to
your add-on’s directory. [Here is Core’s changelog for
reference.](https://github.com/Dreaming381/Latios-Framework-Documentation/blob/main/Core/CHANGELOG.md)
If you do this, you must update your changelog with a new release version with
each pull request you make. You do NOT need to update the root package
changelog. That will be updated upon tagged releases.

## Pull Requests and Support

The final step is to commit your changes and create a pull request. While you
can request for a review over your code if you like, the main purpose of the
pull request review and feedback will be to ensure your add-on conforms to the
inert-by-default rule, and that there is sufficient documentation for a user to
get started (or specify it is still incomplete). Your pull request will not be
accepted until it adheres to these two things.

Once your pull request is accepted, others may start using the add-on, and may
report bugs or other issues. Others may also try and improve it with their own
pull request. You will automatically be assigned all things related to the
add-on and may be directly notified of them through one channel or another. For
any pull request that involves changes to the add-on, you must approve the
changes before the pull request will be accepted. If you are not responsive to
any of these things after 30 days, the add-on will be assumed abandoned, and
then anyone is allowed to make changes without your approval.

Of course, you can also authorize individuals to make changes without the need
for your approval. For example, you could request your add-on be automatically
updated when there are breaking changes made to the framework’s API that have
trivial fixes.

Lastly, if for some reason it is discovered that your add-on was merged, but
accidentally broke the inert-by-default rule, your add-on may be altered or
removed without the need for your approval. Everything is in version control, so
you will be able to revert these changes and fix the issue the proper way when
you have time.

In general, it works best if you state how you’d like to stay involved once you
submit your add-on, and we will do our best to respect that.
