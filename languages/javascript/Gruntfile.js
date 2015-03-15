/*jshint node: true, strict: false */

// -------------------------- grunt -------------------------- //

module.exports = function( grunt ) {

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),

    jshint: {
      src: [ 'hron.js' ],
      options: grunt.file.readJSON('.jshintrc')
    },

    uglify: {
      pkgd: {
        files: {
          'hron.min.js': [ 'hron.js' ]
        },
        options: {
          banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
        }
      }
    }

  });

  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-contrib-uglify');

  grunt.registerTask( 'default', [
    'jshint',
    'uglify'
  ]);

};
