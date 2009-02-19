#!/usr/bin/env python

import sys
sys.path.append("../OpenLayersSource/tools")

import jsmin, mergejs

sourceDirectory = "../OpenLayersSource/lib"
configFilename = "full.cfg"
outputFilename = "../OpenLayers_debug.js"

if len(sys.argv) > 1:
    configFilename = sys.argv[1] + ".cfg"
if len(sys.argv) > 2:
    outputFilename = sys.argv[2]

print "Merging libraries."
merged = mergejs.run(sourceDirectory, None, configFilename)
print "Adding license file."
merged = file("license.txt").read() + merged

print "Writing to %s." % outputFilename
file(outputFilename, "w").write(merged)

print "Done."
