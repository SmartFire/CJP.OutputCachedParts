CJP.OutputCachedParts
=====================

Orchard CMS module that allows you to output cache individual widgets content parts.

Features
========

 * Caches the HTML output of individual drivers to prevent every request from executing driver methods and rendering views
 * Works even when the request is from a logged in user
 * Easy to define custom logic to determine cache keys allows you to cache several variations of output for a single driver
 * Feature switchable- Output caching can be toggled on a per driver basis
 * Designed to extend your current drivers to allow for easy integration into your solution
 * Automatically invalidates cached output when a content item is edited
 * Includes admin UI which allows you to see the cached values and to individually invalidate cache keys
 * Easily extendable to allow you to invalidate cache keys based on your own logic/ events
 * Includes an output cachable version of the **Menu Widget Driver**

Installation
============

 1. Clone the repo into your `Modules` folder.
 2. Enable the `Output Cached Parts` feature. You can also enable the `Output Cached Menu Widget` feature if you want to add output caching to your menu widgets (you will also need to make the change to the `MenuWidgetPartDriver` outlined in step one of `Integration with your current drivers` below).

Once you have installed the module, you will need to integrate it with your current drivers.

Integration with your current drivers
=====================================

Once you have installed the module into your solution, you will have to integrate it with your current drivers. Don't worry, this is easy to do and requires very little effort.

 1. You may have to make a single, unobtrusive change to your current driver. Your current (uncached) driver **must** pass the content part to the dynamic shape factory, and this value must be passed as a property called 'ContentPart'. For example, to cache the `MenuWidgetPartDriver` you will have to update the return statement to pass the part as follows: `return shapeHelper.Parts_MenuWidget(Menu: menuShape, ContentPart: part);`. (This change is required so that the display manager can determine the cache key once it has generated the HTML to be cached.)
 2. Create a new driver that extends and surpresses your original uncached driver. Inject in the `IOutputCachedDriverResultFactory` and override the `Display` method on your base driver to return `_driverResultFactory.BuildResult()`. At it's simplist, this method takes the part, the shape name, and the base display method as a `Func` (for example, the cached menu widget driver could look something like this: `return _driverResultFactory.BuildResult(part, "Parts_MenuWidget", () => base.Display(part, displayType, (object)shapeHelper));`).
 3. `_driverResultFactory.BuildResult()` will also optionally take arguments for a custom cache key and a timepsan object to force the cached value to invalidate after a period of time.
 

 * Take a look at https://github.com/paynecrl97/CJP.OutputCachedParts/blob/master/OutputCachedParts/AlternateImplementations/Drivers/OutputCachedMenuWidgetPartDriver.cs to see an example of how the menu widget is cached and also provides some custom logic to determine the cache key.

Providing global logic to be applied to all of the generated cache keys
=======================================================================

You may want to provide your own logic to create unique cache key variations. For example, you may wish to use different cache keys depending on the day of the week. You can do this by implementing `ICacheKeyCompositeProvider`. This simple interface contains a method that returns a string that will be appended to each of the generated cache keys. This module comes with two of these providers by default- one that appends the current theme, and another that appends the current culture. This means that the cached output is differentitated by both culture and theme.
