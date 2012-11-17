package org.m3.hron;

import java.io.File;
import java.io.IOException;

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/17/12
 * Time: 5:29 PM
 * To change this template use File | Settings | File Templates.
 */
public class Main {
  class EmptyVisitor implements HronVisitor {
    int stringCount = 0;

    @Override
    public Object objectPropertyVisitStarted(Object parent, String propertyName) {
      return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    @Override
    public void objectPropertyVisitEnded(Object parent, String propertyName, Object child) {
      //To change body of implemented methods use File | Settings | File Templates.
    }

    @Override
    public Appendable stringPropertyVisitStarted(Object parent, String propertyName) {
      stringCount++;

      return null;
    }

    @Override
    public void stringPropertyVisitEnded(Object parent, String propertyName, Appendable property) {
      //To change body of implemented methods use File | Settings | File Templates.
    }

    @Override
    public void error(Object parent, long line, int column, HronParseException error) {
      //To change body of implemented methods use File | Settings | File Templates.
    }
  }

  public static void main(String[] args) throws IOException {
    Main instance = new Main();
    instance.run();
  }

  private void run() throws IOException {
    File large = new File("../../reference-data/large.hron");
    HronParser parser = new HronParser();
    EmptyVisitor visitor = new EmptyVisitor();

    long start = System.currentTimeMillis();
    parser.parseFile(large, visitor);

    long delta = System.currentTimeMillis() - start;

    System.out.println("TOOK " + delta + " ms to parse");
    System.out.println("Found " + visitor.stringCount + " strings");

  }
}
