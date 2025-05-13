import Quill from 'quill';

const BlockEmbed = Quill.import('blots/block/embed');

class CustomImage extends BlockEmbed {
  static blotName = 'image';
  static tagName = 'img';

  static create(value: any) {
    const node = super.create();

    if (typeof value === 'string') {
      node.setAttribute('src', value);
    } else {
      node.setAttribute('src', value.src);
      if (value.style) {
        node.setAttribute('style', value.style);
      }
    }

    return node;
  }

  static value(node: any) {
    return {
      src: node.getAttribute('src'),
      style: node.getAttribute('style'),
    };
  }
}

Quill.register(CustomImage);