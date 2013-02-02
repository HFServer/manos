
manosdir=$(bindir)

#pkglib_DATA = $(top_srcdir)/build/manostool.exe
manos_DATA = $(top_srcdir)/build/manostool.exe $(top_srcdir)/build/manostool.exe.mdb

manos_proj_file = manostool.csproj
manos_sources_in = AssemblyInfo.cs.in
#manos_sources = $(manos_proj_file) $(shell  $(XBUILD) /verbosity:minimal /target:ListSources | sed -e '1,3d' -e "s/\;/ /g")
manos_extra_sources = `xbuild /verbosity:minimal /target:ListSources | sed -e '1,3d' -e "s/\;/ /g"`
manos_sources = $(manos_proj_file) $(manos_extra_source)

EXTRA_DIST = $(manos_sources) $(manos_sources_in)
CLEANFILES = $(top_srcdir)/build/manostool.exe


$(top_srcdir)/build/manostool.exe: $(manos_sources)
	$(XBUILD) $(srcdir)/$(manos_proj_file)
